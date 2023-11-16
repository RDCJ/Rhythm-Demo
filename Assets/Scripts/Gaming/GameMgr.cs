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


    Transform judge_line;
    Button pause_btn;
    Button continue_btn;
    Button restart_btn;
    Button back_btn;
    AudioSource audioSource;
    ScoreMgr scoreMgr;
    GameObject pause_panel;
    Text time_txt;

    private MusicCfg music_cfg;
    private List<NoteCfg> composition;

    public string music_id;
    public string difficulty;
    public Action pause_action;
    public Action continue_action;

    public int current_note_idx;
    public int note_count;
    public double current_time;
    public float drop_duration;

    #region statemachine
    public StateMachine stateMachine;
    public InitState initState;
    public PlayingState playingState;
    public PauseState pauseState;
    public ScoreState scoreState;
    public RestartState restartState;
    #endregion

    private void Awake()
    {
        instance = this;
        composition = new List<NoteCfg>();
        judge_line = transform.Find("judge_line");
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
        scoreState = new ScoreState(this, stateMachine);
        restartState = new RestartState(this, stateMachine);

        pause_btn.onClick.AddListener(()=> {
            stateMachine.ChangeState(pauseState);        
        });
        continue_btn.onClick.AddListener(()=> {
            stateMachine.ChangeState(playingState);
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
        music_id = "1";
        difficulty = "Easy";
        drop_duration = (Screen.height - GameConst.judge_line_y) / GameConst.drop_speed;
        stateMachine.Init(initState);
    }

    // Update is called once per frame
    void Update()
    {
        time_txt.text = current_time.ToString("F3");
        stateMachine.CurrentState.FrameUpdate();
    }

    #region logic function
    public void Init()
    {
        current_time = 0;
        current_note_idx = 0;
        pause_btn.gameObject.SetActive(true);
        music_cfg = MusicCfg.GetCfgFromEditor(music_id);

        if (!music_cfg.composition.Keys.Contains(difficulty))
        {
            Debug.Log("difficulty: " + difficulty + " is invalid");
        }
        else
        {
            audioSource.clip = MusicResMgr.GetMusic(int.Parse(music_id));
            audioSource.time = 0;
            audioSource.Pause();

            composition = music_cfg.GetComposition(difficulty);
            note_count = composition.Count;

            scoreMgr.Init(note_count);
        }
    }

    public void DropNote()
    {
        current_time = audioSource.time;
        if (current_note_idx < note_count)
        {
            double next_drop_time = composition[current_note_idx].time - drop_duration;
            if (current_time >= next_drop_time)
            {
                Debug.Log("current_time: " + current_time + " next_drop_time: " + next_drop_time);
                Note.NoteType type = (Note.NoteType)composition[current_note_idx].note_type;
                Note.NoteBase new_note = NotePoolManager.Instance.GetObject(type).GetComponent<Note.NoteBase>();
                new_note.Init(composition[current_note_idx]);
                new_note.Drop();
                current_note_idx++;
            }
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
