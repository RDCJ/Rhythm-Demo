using Note;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorNote : MonoBehaviour, IDragHandler
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
/*        if (cfg.note_type == 3)
        {
            var rect = this.GetComponent<RectTransform>();
            Debug.Log(rect.anchoredPosition);
        }*/

    }

    /// <summary>
    /// 拖动，hold不支持
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (this.cfg.note_type != (int)NoteType.Hold)
        {
            this.cfg = HorizontalGridLine.Instance.GetNoteCfgFromPointer(eventData, this.cfg.note_type);
            CompositionDisplay.Instance.PaintNote(index);
        }
    }
}
