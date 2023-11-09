using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    InputField note_time;
    InputField hold_time;
    Button apply_btn;
    Button create_btn;

    Music.NoteCfg current_note_cfg;

    public void Awake()
    {
        note_selection = transform.Find("note_selection").GetComponent<Dropdown>();
        note_position = transform.Find("note_position").GetComponent<Slider>();
        note_time = transform.Find("note_time").GetComponent<InputField>();
        hold_time = transform.Find("hold_time").GetComponent<InputField>();
        apply_btn = transform.Find("apply_btn").GetComponent<Button>();
        create_btn = transform.Find("create_btn").GetComponent<Button>();

        apply_btn.onClick.AddListener(() => { ChangeNote(); });
        create_btn.onClick.AddListener(() => { CreateNote(); });

        LoadNoteSeletion();
        current_note_cfg = new Music.NoteCfg();
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
        note_time.text = current_note_cfg.time.ToString();
        hold_time.text = current_note_cfg.duration.ToString();
    }

    public void CreateNote()
    {

    }

    public void ChangeNote()
    {

    }

}
