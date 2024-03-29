using Music;
using Note;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorNotePainter : MonoBehaviour, IDragHandler
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public Music.NoteCfg cfg;

    protected Button click_btn;
    protected Outline outline;
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        click_btn = this.GetComponent<Button>();
        outline = this.GetComponentInChildren<Outline>();
        //outline = this.GetComponent<Outline>();
        rectTransform = this.GetComponent<RectTransform>();
        if (click_btn != null)
        {
            click_btn.onClick.AddListener(() => {
                NoteSelector.Instance.LoadNote(index, cfg);
            });
        }
    }

    public void Set(int index, Music.NoteCfg cfg)
    {
        this.index = index;
        this.cfg = cfg;
    }

    private void Update()
    {
        if (outline != null)
            outline.enabled = (NoteSelector.Instance.current_index == index);
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

    /// <summary>
    /// 绘制
    /// </summary>
    public virtual void OnPaint()
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        float x = CompositionDisplay.Instance.WindowWidth * (float)cfg.FirstCheckPoint().Center();
        float y = drop_speed * (float)(cfg.FirstCheckPoint().time - CompositionEditor.Instance.GetTimeOffset) - CompositionDisplay.Instance.ContentHeight;
        rectTransform.localPosition = new Vector3(x, y, 0);
    }
}
