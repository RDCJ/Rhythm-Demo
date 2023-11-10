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
    Image img;
    Color origin_color;
    Color select_color;

    private void Awake()
    {
        click_btn = this.GetComponent<Button>();
        img = this.GetComponent<Image>();
        click_btn.onClick.AddListener(() => {
            NoteEditor.Instance.LoadNote(index, cfg);
        });
        origin_color = img.color;
        select_color = new Color(0, 255, 255);
    }

    public void Set(int index, Music.NoteCfg cfg)
    {
        this.index = index;
        this.cfg = cfg;
    }

    private void Update()
    {
        if (NoteEditor.Instance.current_index == index)
            img.color = select_color;
        else
            img.color = origin_color;
    }
}
