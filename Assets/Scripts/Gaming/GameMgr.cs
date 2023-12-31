using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Music;
using System;
using UnityEngine.UI;

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
    /// 当前歌曲的id
    /// </summary>
    public string music_id;
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
        pause_btn = transform.Find("pause_btn").GetComponent<Button>();
        pause_panel = transform.Find("pause_panel").gameObject;
        continue_btn = pause_panel.transform.Find("continue_btn").GetComponent<Button>();
        restart_btn = pause_panel.transform.Find("restart_btn").GetComponent<Button>();
        back_btn = pause_panel.transform.Find("back_btn").GetComponent<Button>();
        time_txt = transform.Find("time_txt").GetComponent<Text>();

        audioSource = transform.Find("music").GetComponent<AudioSource>();
        scoreMgr = transform.Find("score_mgr").GetComponent<ScoreMgr>();

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
            Destroy(this.gameObject);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        drop_duration = Screen.height * (1 - GameConst.judge_line_y) / DropSpeedFix.GetScaledDropSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        time_txt.text = current_time.ToString("N2") + " / " + audioSource.clip.length.ToString("N2");
        stateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }

    #region logic function
    public void SetMusic(string music_id, string difficulty)
    {
        this.music_id = music_id;
        this.difficulty = difficulty;
        music_cfg = MusicCfg.GetCfg(music_id);
        music_cfg.prepare_time = Math.Max(0, music_cfg.prepare_time);
        stateMachine.Init(initState);
    }

    public void Init()
    {
        pause_btn.gameObject.SetActive(true);
        pause_panel.gameObject.SetActive(false);
        current_time = -music_cfg.prepare_time;
        current_note_idx = 0;

        audioSource.clip = MusicResMgr.GetMusic(int.Parse(music_id));
        audioSource.time = 0;
        audioSource.Stop();
        if (!music_cfg.composition.Keys.Contains(difficulty))
        {
            Debug.Log("difficulty: " + difficulty + " is invalid");
        }
        else
        {
            composition = music_cfg.GetComposition(difficulty);
            note_count = composition.Count;

            scoreMgr.Init(note_count);
        }
    }

    public void GenerateNote()
    {
        current_time += Time.deltaTime;
        if (IsNoteEnd) 
            return;
        while (true)
        {
            if (IsNoteEnd) break;
            double next_drop_time = composition[current_note_idx].time - drop_duration;
            if (current_time < next_drop_time) break;

            //Debug.Log("current_time: " + current_time + " next_drop_time: " + next_drop_time);
            Note.NoteType type = (Note.NoteType)composition[current_note_idx].note_type;
            Note.NoteBase new_note = NotePoolManager.Instance.GetObject(type).GetComponent<Note.NoteBase>();
            new_note.Init(composition[current_note_idx], (float)(next_drop_time - current_time));
            //if (type != Note.NoteType.Hold)
            new_note.Drop();
            current_note_idx++;
        }
    }

    public void Pause()
    {
        audioSource.Pause();
        pause_panel.SetActive(true);
        pause_action?.Invoke();
    }

    public void Continue()
    {
        audioSource.Play();
        pause_panel.SetActive(false);
        continue_action?.Invoke();
    }
    #endregion
}
