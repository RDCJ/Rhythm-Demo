using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorNote : MonoBehaviour
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public Music.NoteCfg cfg;

    Button click_btn;
    Outline outline;

    private void Awake()
    {
        click_btn = this.GetComponent<Button>();
        outline = this.GetComponent<Outline>();
        click_btn.onClick.AddListener(() => {
            NoteEditor.Instance.LoadNote(index, cfg);
        });
    }

    public void Set(int index, Music.NoteCfg cfg)
    {
        this.index = index;
        this.cfg = cfg;
    }

    private void Update()
    {
        outline.enabled = (NoteEditor.Instance.current_index == index);
    }
}
