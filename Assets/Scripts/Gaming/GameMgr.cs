using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Music;
using System;
using UnityEngine.UI;
using GestureEvent;

public class GameMgr : MonoBehaviour
{
    #region Singleton
    private GameMgr() { }
    private static GameMgr instance;
    public static GameMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region component
    Transform GameCanvas_tf;
    Transform UICanvas_tf;
    Transform BGCanvas_tf;

    public AudioSource audioSource;
    public MusicBackground musicBackground;
    public GestureEvent.GestureMgr gestureMgr;
    #endregion

    public ScoreMgr scoreMgr { get; private set; }

    private MusicCfg music_cfg;
    private List<NoteCfg> composition;
    /// <summary>
    /// 游戏初始化时调用action
    /// </summary>
    public Action OnGameInit;
    /// <summary>
    /// 游戏暂停时调用action
    /// </summary>
    public Action OnGamePause;
    /// <summary>
    /// 游戏继续时调用action
    /// </summary>
    public Action OnGameContinue;
    /// <summary>
    /// 游戏结束时调用action
    /// </summary>
    public Action<GameResultScore> OnGameEnd;
    /// <summary>
    /// 触发计分时调用action
    /// </summary>
    public Action<ScoreMgr.ScoreLevel, GameResultScore> OnAddScore;
    /// <summary>
    /// 当前加载的音乐文件名
    /// </summary>
    public string music_file_name;
    /// <summary>
    /// 当前选择的难度
    /// </summary>
    public string difficulty;
    /// <summary>
    /// 下一个将要生成的note的index
    /// </summary>
    public int current_note_idx;
    /// <summary>
    /// 曲谱包含的note数量
    /// </summary>
    public int note_count;
    /// <summary>
    /// 当前游戏时间
    /// </summary>
    public double prepare_time;
    /// <summary>
    /// note从生成点移动到判定线所需时间
    /// </summary>
    public float drop_duration;
    /// <summary>
    /// 当前游戏时间
    /// </summary>
    public double CurrentTime
    {
        get
        {
            if (prepare_time < 0)
                return prepare_time;
            else
                return audioSource.time;
        }
    }

    public bool is_test_mode;
    float test_mode_start_time;

    #region statemachine
    public StateMachine stateMachine;
    public InitState initState;
    public PrepareState prepareState;
    public PlayingState playingState;
    public PauseState pauseState;
    public RestartState restartState;
    public MusicEndState musicEndState;
    public PrepareTestState prepareTestState;
    #endregion

    public bool IsMusicEnd
    {
        get => (CurrentTime >= audioSource.clip.length) || (IsNoteEnd && CurrentTime == 0);
    }

    public bool IsNoteEnd
    {
        get => current_note_idx >= note_count;
    }

    public bool IsGamePause
    {
        get; private set;
    }

    private void Awake()
    {
        instance = this;
        scoreMgr = new ScoreMgr();
        composition = new List<NoteCfg>();
        audioSource = transform.Find("music").GetComponent<AudioSource>();

        GameCanvas_tf = transform.Find("GameCanvas");
        UICanvas_tf = transform.Find("UICanvas");
        BGCanvas_tf = transform.Find("BGCanvas");
        
        musicBackground = BGCanvas_tf.GetComponent<MusicBackground>();
        gestureMgr = GameCanvas_tf.Find("GestureMgr").GetComponent<GestureMgr>();
        
        stateMachine = new StateMachine();
        initState = new InitState(this, stateMachine);
        playingState = new PlayingState(this, stateMachine);
        pauseState = new PauseState(this, stateMachine);
        restartState = new RestartState(this, stateMachine);
        prepareState = new PrepareState(this, stateMachine);
        musicEndState = new MusicEndState(this, stateMachine);
        prepareTestState = new PrepareTestState(this, stateMachine);
    }


    // Update is called once per frame
    void Update()
    {
        stateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }

    #region logic function
    public void StartInitGame(string music_file_name, string difficulty, bool test_mode=false, float start_time=0)
    {
        this.gameObject.SetActive(true);
        CloseAllCanvas();
        this.music_file_name = music_file_name;
        this.difficulty = difficulty;
        this.is_test_mode = test_mode;
        this.test_mode_start_time = start_time;
        music_cfg = MusicResMgr.GetCfg(music_file_name);
        music_cfg.prepare_time = Math.Max(0, music_cfg.prepare_time);
        stateMachine.Init(initState);
    }

    /// <summary>
    /// 初始化，加载音乐和谱面
    /// </summary>
    public void Init()
    {
        // 加载谱面配置
        var _composition = music_cfg.GetComposition(difficulty);
        if (_composition.Count <= 0)
        {
            Debug.Log("difficulty: " + difficulty + " is invalid");
            LoadingScreenManager.Instance.EndLoading(_onFinishIdle: ()=> Close());
        }
        else
        {
            OpenAllCanvas();
            // 加载谱面
            composition = _composition;
            note_count = composition.Count;
            current_note_idx = 0;
            // 初始化计分
            scoreMgr.Init(note_count);

            drop_duration = (Screen.height / 2 - JudgeLine.localPositionY) / PlayerPersonalSetting.ScaledDropSpeed;
            Debug.Log("下落速度: " + PlayerPersonalSetting.ScaledDropSpeed);
            Debug.Log("下落时间: " + drop_duration);

            gestureMgr.enabled = false;
            OnGameInit?.Invoke();

            // 加载音乐和背景
            WaitForAllCoroutine loading_task = new WaitForAllCoroutine(this);
            loading_task.AddCoroutine(MusicResMgr.GetMusic(this.music_file_name, (AudioClip clip) =>
            {
                Debug.Log("finish loading music, Time.time: " + Time.time);
                audioSource.clip = clip;
                audioSource.time = 0;
                audioSource.Stop();
            }))
            .AddCoroutine(musicBackground.Init(this.music_file_name))
            .StartAll(() => {
                LoadingScreenManager.Instance.EndLoading(_onFinishHide: 
                    () => {
                        if (is_test_mode)
                            stateMachine.ChangeState(prepareTestState);
                        else
                            stateMachine.ChangeState(prepareState); 
                    }
                );
            });
        }
    }

    public void GenerateNote()
    {
        while (true)
        {
            if (IsNoteEnd) break;
            double next_drop_time = composition[current_note_idx].FirstCheckPoint().time - drop_duration;
            if (CurrentTime < next_drop_time) break;

            Debug.Log("[GameMgr.GenerateNote] success: current_time: " + CurrentTime + " std_drop_time: " + next_drop_time);
            Note.NoteType type = (Note.NoteType)composition[current_note_idx].note_type;
            Note.NoteBase new_note = NotePoolManager.Instance.GetObject(type).GetComponent<Note.NoteBase>();
            new_note.Init(composition[current_note_idx], (float)(next_drop_time - CurrentTime), gestureMgr);
#if UNITY_EDITOR
            if (type == Note.NoteType.Hold)
            {
                if (GameConst.hold_note_drop)
                    new_note.Drop();
            }
            else
            {
                if (GameConst.note_drop)
                    new_note.Drop();
            }
#else
            new_note.Drop();
#endif
            current_note_idx++;
        }
    }

    public void Pause()
    {
        IsGamePause = true;
        audioSource.Pause();
        musicBackground.Pause();
        OnGamePause?.Invoke();
        gestureMgr.enabled = false;
    }

    public void Continue()
    {
        IsGamePause = false;
        audioSource.Play();
        musicBackground.Play();
        OnGameContinue?.Invoke();
        gestureMgr.enabled = true;
    }
#endregion

    private void LateUpdate()
    {
        stateMachine.CurrentState.FrameLateUpdate();
    }

    public void Close()
    {
        audioSource.Stop();
        this.gameObject.SetActive(false);
        NotePoolManager.Instance.Reload();
        MusicSelect.Instance?.RefreshRecord();
    }

    public void CloseAllCanvas()
    {
        UICanvas_tf.gameObject.SetActive(false);
        BGCanvas_tf.gameObject.SetActive(false);
        GameCanvas_tf.gameObject.SetActive(false);
    }

    public void OpenAllCanvas()
    {
        UICanvas_tf.gameObject.SetActive(true);
        BGCanvas_tf.gameObject.SetActive(true);
        GameCanvas_tf.gameObject.SetActive(true);
    }

    public void CalcPrepareTime()
    {
/*        float first_note_time = (composition.Count > 0 && !IsNoteEnd) ? (float)composition[current_note_idx].FirstCheckPoint().time : float.MaxValue;
        prepare_time = 2 * drop_duration;//Mathf.Min(first_note_time - drop_duration, 0); */

        float first_note_time = composition.Count > 0 ? (float)composition[0].FirstCheckPoint().time : float.MaxValue;
        prepare_time = Mathf.Min(first_note_time - drop_duration, 0);
    }

    public void PrepareTestMode()
    {
        if (test_mode_start_time <= 0) return;

        while (true)
        {
            if (IsNoteEnd) break;
            double next_note_time = composition[current_note_idx].FirstCheckPoint().time;
            if (next_note_time < test_mode_start_time)
                current_note_idx++;
            else
                break;
        }

        audioSource.time = Mathf.Max(0, (float)composition[current_note_idx].FirstCheckPoint().time - 2 * drop_duration);
    }

    public void AddScore(ScoreMgr.ScoreLevel scoreLevel)
    {
        scoreMgr.AddScore(scoreLevel);
        OnAddScore?.Invoke(scoreLevel, scoreMgr.gameResultScore);
    }

    public void EnterMusicEnd()
    {
        audioSource.Stop();
        musicBackground.Stop();
        scoreMgr.OnMusicEnd();
        OnGameEnd?.Invoke(scoreMgr.gameResultScore);
        gestureMgr.enabled = false;
    }
}
