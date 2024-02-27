using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Music;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

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
    Button pause_btn;
    Button continue_btn;
    Button restart_btn;
    Button back_btn;
    public AudioSource audioSource;
    ScoreMgr scoreMgr;
    GameObject pause_panel;
    Text time_txt;
    Image bg_img;
    Texture2D bg_tex;
    GameObject bg_video;
    RawImage bg_videoRawImage;
    VideoPlayer bg_videoPlayer;
    #endregion

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
    public double current_time;
    /// <summary>
    /// note从生成点移动到判定线所需时间
    /// </summary>
    public float drop_duration;

    #region statemachine
    public StateMachine stateMachine;
    public InitState initState;
    public PrepareState prepareState;
    public PlayingState playingState;
    public PauseState pauseState;
    public RestartState restartState;
    public MusicEndState musicEndState;
    #endregion

    public MusicCfg GetMusicCfg
    {
        get { return music_cfg; }
    }

    public bool IsMusicEnd
    {
        get => current_time >= audioSource.clip.length;
    }

    public bool IsNoteEnd
    {
        get => current_note_idx >= note_count;
    }

    private void Awake()
    {
        instance = this;
        composition = new List<NoteCfg>();
        audioSource = transform.Find("music").GetComponent<AudioSource>();

        Transform GameCanvas_tf = transform.Find("GameCanvas");
        Transform UICanvas_tf = transform.Find("UICanvas");
        pause_btn = UICanvas_tf.Find("pause_btn").GetComponent<Button>();
        pause_panel = UICanvas_tf.Find("pause_panel").gameObject;
        time_txt = UICanvas_tf.Find("time_txt").GetComponent<Text>();
        scoreMgr = UICanvas_tf.Find("score_mgr").GetComponent<ScoreMgr>();

        continue_btn = pause_panel.transform.Find("btn/continue_btn").GetComponent<Button>();
        restart_btn = pause_panel.transform.Find("btn/restart_btn").GetComponent<Button>();
        back_btn = pause_panel.transform.Find("btn/back_btn").GetComponent<Button>();

        bg_img = transform.Find("BGCanvas/ImgBG").GetComponent<Image>();
        bg_video = transform.Find("BGCanvas/VideoBG").gameObject;
        bg_videoPlayer = transform.Find("BGCanvas/VideoBG/VideoPlayer").GetComponent<VideoPlayer>();
        bg_videoRawImage = transform.Find("BGCanvas/VideoBG/RawImage").GetComponent<RawImage>();
        
        stateMachine = new StateMachine();
        initState = new InitState(this, stateMachine);
        playingState = new PlayingState(this, stateMachine);
        pauseState = new PauseState(this, stateMachine);
        restartState = new RestartState(this, stateMachine);
        prepareState = new PrepareState(this, stateMachine);
        musicEndState = new MusicEndState(this, stateMachine);

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
    }


    // Update is called once per frame
    void Update()
    {
        stateMachine.CurrentState.FrameUpdate();
        if (audioSource.clip != null)
        {
            time_txt.text = current_time.ToString("N2") + " / " + audioSource.clip.length.ToString("N2");
        }
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }

    #region logic function
    public void StartInitGame(string music_file_name, string difficulty)
    {
        this.gameObject.SetActive(true);
        this.music_file_name = music_file_name;
        this.difficulty = difficulty;
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
        if (!music_cfg.composition.ContainsKey(difficulty))
        {
            Debug.Log("difficulty: " + difficulty + " is invalid");
            Close();
        }
        else
        {
            // 加载谱面
            composition = music_cfg.GetComposition(difficulty);
            note_count = composition.Count;
            // 初始化计分
            scoreMgr.Init(note_count);
            JudgeLine.Instance.Reset();
            drop_duration = (Screen.height / 2 - JudgeLine.localPositionY) / DropSpeedFix.GetScaledDropSpeed;
            Debug.Log("下落速度: " + DropSpeedFix.GetScaledDropSpeed);
            Debug.Log("下落时间: " + drop_duration);


            float first_note_time = composition.Count > 0 ? (float)composition[0].checkPoints[0].time : float.MaxValue;
            current_time = Mathf.Min(first_note_time - drop_duration, 0);

            // 加载音乐和背景
            WaitForAllCoroutine loading_task = new WaitForAllCoroutine(this);
            loading_task.AddCoroutine(MusicResMgr.GetMusic(this.music_file_name, (AudioClip clip) =>
            {
                Debug.Log("finish loading music, Time.time: " + Time.time);
                audioSource.clip = clip;
                audioSource.time = 0;
                audioSource.Stop();
            }));

            if (MusicResMgr.BGIsVideo(this.music_file_name))
            {
                bg_videoPlayer.url = MusicResMgr.GetBGFilePath(this.music_file_name);
                bg_img.gameObject.SetActive(false);
                bg_video.SetActive(true);
            }
            else
            {
                loading_task.AddCoroutine(MusicResMgr.GetBG(this.music_file_name, (Texture2D tex) =>
                {
                    if (bg_tex != null)
                        Destroy(bg_tex);
                    if (tex != null)
                    {
                        bg_tex = tex;
                        bg_img.sprite = Sprite.Create(bg_tex, new Rect(0, 0, bg_tex.width, bg_tex.height), new Vector2(0.5f, 0.5f));
                        bg_img.color = Color.white;
                    }
                    else
                    {
                        bg_img.sprite = null;
                        bg_img.color = Color.black;
                    }

                }));

                bg_img.gameObject.SetActive(true);
                bg_video.SetActive(false);
            }
            loading_task.StartAll(() =>{
                Util.DelayOneFrame(this, () =>
                {
                        stateMachine.ChangeState(prepareState);
                });
            });
        }

        
    }

    public void GenerateNote()
    {
        if (current_time < 0)
            current_time += Time.deltaTime;
        else
            current_time = audioSource.time;
/*        if (current_note_idx == 0)
            Debug.Log(Time.deltaTime);*/
        if (IsNoteEnd) 
            return;
        while (true)
        {
            if (IsNoteEnd) break;
            double next_drop_time = composition[current_note_idx].FirstCheckPoint().time - drop_duration;
            if (current_time < next_drop_time) break;

            Debug.Log("current_time: " + current_time + " next_drop_time: " + next_drop_time);
            Note.NoteType type = (Note.NoteType)composition[current_note_idx].note_type;
            Note.NoteBase new_note = NotePoolManager.Instance.GetObject(type).GetComponent<Note.NoteBase>();
            new_note.Init(composition[current_note_idx], (float)(next_drop_time - current_time));
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
        audioSource.Pause();
        bg_videoPlayer.Pause();
        pause_panel.SetActive(true);
        pause_action?.Invoke();
    }

    public void Continue()
    {
        audioSource.Play();
        bg_videoPlayer.Play();
        pause_panel.SetActive(false);
        continue_action?.Invoke();
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
}
