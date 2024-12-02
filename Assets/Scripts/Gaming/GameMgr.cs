using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Music;
using System;
using UnityEngine.UI;
using GestureEvent;
using static ScoreMgr;

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

    GameObject pause_panel;
    Button pause_btn;
    Button continue_btn;
    Button restart_btn;
    Button back_btn;
    
    
    ScoreUI scoreUI;
    Text time_txt;
    Slider time_progress;

    public AudioSource audioSource;
    public MusicBackground musicBackground;
    public GestureEvent.GestureMgr gestureMgr;
    #endregion

    public ScoreMgr scoreMgr { get; private set; }

    private MusicCfg music_cfg;
    private List<NoteCfg> composition;

    /// <summary>
    /// 游戏暂停时调用action
    /// </summary>
    public Action pause_action;
    /// <summary>
    /// 游戏继续时调用action
    /// </summary>
    public Action continue_action;
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

    public MusicCfg GetMusicCfg
    {
        get { return music_cfg; }
    }

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
        pause_btn = UICanvas_tf.Find("pause_btn").GetComponent<Button>();
        pause_panel = UICanvas_tf.Find("pause_panel").gameObject;
        time_txt = UICanvas_tf.Find("time_txt").GetComponent<Text>();
        time_progress = UICanvas_tf.Find("time_progress").GetComponent<Slider>();
        scoreUI = UICanvas_tf.Find("ScoreUI").GetComponent<ScoreUI>();

        continue_btn = pause_panel.transform.Find("btn/continue_btn").GetComponent<Button>();
        restart_btn = pause_panel.transform.Find("btn/restart_btn").GetComponent<Button>();
        back_btn = pause_panel.transform.Find("btn/back_btn").GetComponent<Button>();

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

        pause_btn.onClick.AddListener(()=> {
            stateMachine.ChangeState(pauseState);        
        });
        continue_btn.onClick.AddListener(()=> {
            stateMachine.ChangeState(stateMachine.LastState);
        });
        restart_btn.onClick.AddListener(() => {
            stateMachine.ChangeState(restartState);
        });
        back_btn.onClick.AddListener(() =>{
            Close();
        });

#if UNITY_EDITOR
        time_txt.gameObject.SetActive(true);
#else
        time_txt.gameObject.SetActive(false);
#endif
    }


    // Update is called once per frame
    void Update()
    {
        stateMachine.CurrentState.FrameUpdate();
        if (audioSource.clip != null)
        {
            time_txt.text = CurrentTime.ToString("N2") + " / " + audioSource.clip.length.ToString("N2");
            time_progress.value = MathF.Max(0, (float)CurrentTime) / audioSource.clip.length;
        }
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
        pause_btn.gameObject.SetActive(true);
        pause_panel.gameObject.SetActive(false);
        current_note_idx = 0;

        // 加载资源
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
            // 初始化计分
            scoreMgr.Init(note_count);
            scoreUI.Init();
            JudgeLine.Instance.Reset();
            drop_duration = (Screen.height / 2 - JudgeLine.localPositionY) / PlayerPersonalSetting.ScaledDropSpeed;
            Debug.Log("下落速度: " + PlayerPersonalSetting.ScaledDropSpeed);
            Debug.Log("下落时间: " + drop_duration);

            gestureMgr.enabled = false;

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
        pause_panel.SetActive(true);
        pause_action?.Invoke();
        gestureMgr.enabled = false;
    }

    public void Continue()
    {
        IsGamePause = false;
        audioSource.Play();
        musicBackground.Play();
        pause_panel.SetActive(false);
        continue_action?.Invoke();
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
        scoreUI.OnAddScore(scoreMgr.gameResultScore);
        if (scoreMgr.gameResultScore.score_level_count[(int)ScoreLevel.bad] > 0)
        {
            JudgeLine.Instance.ChangeColor(0);
        }
        else if (scoreMgr.gameResultScore.score_level_count[(int)ScoreLevel.good] > 0)
        {
            JudgeLine.Instance.ChangeColor(1);
        }
    }

    public void EnterMusicEnd()
    {
        audioSource.Stop();
        musicBackground.Stop();
        scoreMgr.OnMusicEnd();
        scoreUI.ShowFinalScore(scoreMgr.gameResultScore);
        gestureMgr.enabled = false;
    }
}
