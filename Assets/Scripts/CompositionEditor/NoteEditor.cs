using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Music;
using Note;

public class NoteEditor : MonoBehaviour
{
    #region Singleton
    private NoteEditor() { }
    private static NoteEditor instance;
    public static NoteEditor Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    Dropdown note_selection;
    InputField hold_time;
    

    public int current_index;
    NoteCfg current_note_cfg;

    public void Awake()
    {
        instance = this;
        note_selection = transform.Find("note_selection").GetComponent<Dropdown>();
        hold_time = transform.Find("hold_time").GetComponent<InputField>();
        

        note_selection.onValueChanged.AddListener((int value) =>
        {
            Debug.Log("select " + value);
            hold_time.gameObject.SetActive(value == (int)NoteType.Hold);
        });
        

        LoadNoteSeletion();
        current_index = -1;
        current_note_cfg = new NoteCfg();
    }

    private void Start()
    {
        hold_time.gameObject.SetActive(note_selection.value == (int)NoteType.Hold);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
            CreateNote();
        if (Input.GetKeyDown(KeyCode.Delete))
            DeleteNote();
    }

    /// <summary>
    /// 初始化note选项
    /// </summary>
    private void LoadNoteSeletion()
    {
        string[] note_name =
        {
            "Tap", "LeftSlide", "RightSlide", "Hold"
        };
        foreach (string name in note_name)
            note_selection.options.Add(new Dropdown.OptionData(name));
    }

    public void RefreshEditor()
    {
        note_selection.value = current_note_cfg.note_type;
        if (current_note_cfg.note_type == (int)NoteType.Hold)
        {
            hold_time.gameObject.SetActive(true);
            hold_time.text = current_note_cfg.duration.ToString();
        }
        else
        {
            hold_time.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateNote()
    {
        ToCfg();
        current_index = CompositionDisplay.Instance.CreateNewNote(current_note_cfg);
    }

    /// <summary>
    /// 删除当前选中的note
    /// </summary>
    public void DeleteNote()
    {
        if (current_index != -1)
        {
            CompositionDisplay.Instance.DeleteNote(current_index);
            current_index = -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetNote()
    {
        if (current_index != -1)
        {
            DeleteNote();
            CreateNote();
        }
    }



    public void LoadNote(int index, NoteCfg cfg)
    {
        current_index = index;
        current_note_cfg = cfg;
        RefreshEditor();
    }

    private void ToCfg()
    {
        current_note_cfg = new NoteCfg();
        current_note_cfg.note_type = note_selection.value;
        current_note_cfg.position_x = 0.5;
        current_note_cfg.time = AudioWaveForm.Instance.GetCurrentAudioTime;
        current_note_cfg.duration = 0;
        if (current_note_cfg.note_type == (int)NoteType.Hold)
            current_note_cfg.duration = GetDuration;
    }

    public int GetNoteType
    {
        get => note_selection.value;
    }

    public float GetDuration
    {
        get => float.Parse(hold_time.text);
    }
}