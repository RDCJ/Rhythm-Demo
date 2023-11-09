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
    Slider note_position;
    TimeWidget note_time;
    Button set_time_btn;
    TimeWidget hold_time;
    Button apply_btn;
    Button create_btn;
    Button delete_btn;

    NoteCfg current_note_cfg;

    public void Awake()
    {
        instance = this;
        note_selection = transform.Find("note_selection").GetComponent<Dropdown>();
        note_position = transform.Find("note_position").GetComponent<Slider>();
        note_time = transform.Find("note_time").GetComponent<TimeWidget>();
        hold_time = transform.Find("hold_time").GetComponent<TimeWidget>();
        apply_btn = transform.Find("apply_btn").GetComponent<Button>();
        create_btn = transform.Find("create_btn").GetComponent<Button>();
        delete_btn = transform.Find("delete_btn").GetComponent<Button>();
        set_time_btn = transform.Find("set_time_btn").GetComponent<Button>();

        apply_btn.onClick.AddListener(() => { ChangeNote(); });
        create_btn.onClick.AddListener(() => { CreateNote(); });
        delete_btn.onClick.AddListener(() => {  DeleteNote(); });

        set_time_btn.onClick.AddListener(() => {
            note_time.SetTime(new Times(AudioWaveForm.Instance.GetCurrentAudioTime));
        });

        LoadNoteSeletion();
        current_note_cfg = new NoteCfg();
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
        note_position.value = (float)current_note_cfg.position_x;
        note_time.SetTime(new Times((float)current_note_cfg.time));
        if (current_note_cfg.note_type == (int)NoteType.Hold)
        {
            hold_time.gameObject.SetActive(true);
            hold_time.SetTime(new Times((float)current_note_cfg.duration));
        }
        else 
        {
            hold_time.gameObject.SetActive(false);
        }
        
    }

    public void CreateNote()
    {

    }

    public void ChangeNote()
    {

    }

    public void DeleteNote()
    {

    }

}
