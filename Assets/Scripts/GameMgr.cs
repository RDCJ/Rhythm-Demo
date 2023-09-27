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

    public enum GameStatus
    {
        Init,
        Running,
        Pause,
        NoteEnd,
        MusicEnd
    }

    Transform judge_line;
    Button pause_btn;
    Button continue_btn;
    public Text text;

    private MusicCfg music_cfg;
    private List<NoteCfg> composition;
    private GameStatus game_status;
    public GameStatus GetGameStatus
    {
        get => game_status;
    }
    public string level;
    public Action pause_action;
    public Action continue_action;

    private int current_note_idx;
    private int note_count;
    private double current_time;
    private float drop_time = (Screen.height - GameConst.judge_line_y) / GameConst.drop_speed;

    private void Awake()
    {
        instance = this;
        composition = new List<NoteCfg>();
        judge_line = transform.Find("judge_line");
        pause_btn = transform.Find("pause_btn").GetComponent<Button>();
        continue_btn = transform.Find("continue_btn").GetComponent<Button>();
        text = transform.Find("Text").GetComponent<Text>();
        pause_btn.onClick.AddListener(Pause);
        continue_btn.onClick.AddListener(Continue);
    }

    // Start is called before the first frame update
    void Start()
    {
        pause_btn.gameObject.SetActive(true);
        continue_btn.gameObject.SetActive(false);
        Init("1", "easy");
        
       // StartCoroutine(Running());
    }

    // Update is called once per frame
    void Update()
    {
        if (game_status == GameStatus.Running && current_note_idx < note_count)
        {
            current_time += Time.deltaTime;
            double next_drop_time = composition[current_note_idx].time - drop_time;
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
        if (current_note_idx >= note_count)
            game_status = GameStatus.NoteEnd;
    }

    public void Init(string music_id, string _level)
    {
        current_note_idx = 0;
        game_status = GameStatus.Init;
        music_cfg = MusicCfg.GetCfg(MusicCfg.GetCfgNameByID(music_id));
        text.gameObject.SetActive(true);
        level = _level;
        if (!music_cfg.composition.Keys.Contains(level))
        {
            Debug.Log("Level: " + _level + " is invalid");
        }
        else
        {
            composition.Clear();
            for (int i = 0; i < music_cfg.composition[level].Count; i++)
                composition.Add(JsonUtility.FromJson<NoteCfg>(music_cfg.composition[level][i].ToJson()));
            note_count = composition.Count;
        }
        game_status = GameStatus.Running;
        current_time = 0;
    }

    private IEnumerator Running()
    {
        if (note_count > 0)
        {
            game_status = GameStatus.Running;
            int note_idx = 0;
            double current_time = 0;
            double next_drop_time = composition[note_idx].time - drop_time;

            while (note_idx < note_count)
            {
                if (game_status != GameStatus.Running)
                    yield return new WaitWhile(() => game_status != GameStatus.Running);

                while (note_idx < note_count && current_time >= next_drop_time)
                {
                    Debug.Log("current_time: " + current_time + " next_drop_time: " + next_drop_time);
                    Note.NoteType type = (Note.NoteType)composition[note_idx].note_type;
                    Note.NoteBase new_note = NotePoolManager.Instance.GetObject(type).GetComponent<Note.NoteBase>();
                    new_note.Init(composition[note_idx]);
                    new_note.Drop();
                    note_idx++;
                    if (note_idx < note_count)
                        next_drop_time = composition[note_idx].time - drop_time;
                }
                current_time += Time.deltaTime;
                yield return null;
            }
        }
        game_status = GameStatus.NoteEnd;
    }

    private void Pause()
    {
        game_status = GameStatus.Pause;
        pause_btn.gameObject.SetActive(false);
        continue_btn.gameObject.SetActive(true);
        pause_action?.Invoke();
    }
    private void Continue()
    {
        game_status = GameStatus.Running;
        pause_btn.gameObject.SetActive(true);
        continue_btn.gameObject.SetActive(false);
        continue_action?.Invoke();
    }
}
