using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Music;
using Note;
using UnityEngine.EventSystems;

public class NoteSelector : MonoBehaviour
{
    #region Singleton
    private NoteSelector() { }
    private static NoteSelector instance;
    public static NoteSelector Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    Dropdown note_selection;

    public int current_index;
    NoteCfg current_note_cfg;

    public void Awake()
    {
        instance = this;
        note_selection = transform.Find("note_selection").GetComponent<Dropdown>();

        LoadNoteSeletion();
        current_index = -1;
        current_note_cfg = new NoteCfg();
    }


    private void Update()
    {
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
            "Tap", "LeftSlide", "RightSlide", "Hold", "Catch"
        };
        foreach (string name in note_name)
            note_selection.options.Add(new Dropdown.OptionData(name));
    }

    public void Refresh()
    {
        note_selection.value = current_note_cfg.note_type;
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

    public void LoadNote(int index, NoteCfg cfg)
    {
        current_index = index;
        current_note_cfg = cfg;
        Refresh();
    }

    public int GetNoteType
    {
        get => note_selection.value;
    }

    public string GetNoteTypeStr
    {
        get => note_selection.options[note_selection.value].text;
    }
}