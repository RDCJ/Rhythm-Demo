using Music;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorHoldPainter : EditorNotePainter
{
    GameObject checkPointDrag;
    HoldPolygonImage hold_icon;

    List<CheckPointDrag> center_drags;

    protected override void Awake()
    {
        base.Awake();
        hold_icon = this.GetComponent<HoldPolygonImage>();
        checkPointDrag = this.transform.Find("checkpoint_drag").gameObject;
        center_drags = new List<CheckPointDrag>();
    }

    public override void OnPaint()
    {
        RePaint();
        SetCheckPointDrag();
    }

    private void RePaint()
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        PaintHold(rectTransform, cfg);

        float x = 0;
        float y = drop_speed * (float)(cfg.FirstCheckPoint().time - CompositionEditor.Instance.GetTimeOffset) - CompositionDisplay.Instance.ContentHeight;
        y += rectTransform.sizeDelta.y * rectTransform.localScale.y / 2.0f;

        rectTransform.anchoredPosition = Util.ChangeV2(rectTransform.anchoredPosition, 0, x);
        rectTransform.localPosition = Util.ChangeV3(rectTransform.localPosition, 1, y);
    }


    public static void PaintHold(RectTransform note_trans, NoteCfg cfg)
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        var hold_icon = note_trans.GetComponent<HoldPolygonImage>();
        hold_icon.SetCheckPoints(cfg.checkPoints, drop_speed, CompositionDisplay.Instance.WindowWidth);

        var head_handle = note_trans.Find("head_handle") as RectTransform;
        var tail_handle = note_trans.Find("tail_handle") as RectTransform;
        head_handle.localPosition = hold_icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, hold_icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = hold_icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, hold_icon.TailWidth / tail_handle.localScale.x);
    }

    private void SetCheckPointDrag()
    {
        for (int i = 1; i <= hold_icon.checkpoint_count; i++)
        {
            CheckPointDrag drag;
            if (center_drags.Count < i)
            {
                drag = Instantiate(checkPointDrag, transform).GetComponent<CheckPointDrag>();
                drag.index = i - 1;
                center_drags.Add(drag);
            }
            else
                drag = center_drags[i - 1];

            drag.gameObject.SetActive(true);
            RectTransform drag_tf = drag.rectTransform;
            drag_tf.localPosition = hold_icon.GetCheckPointCenter(i);
            
        }
    }

    public void OnCheckPointDrag(PointerEventData eventData, int index, RectTransform drag_tf)
    {
        double time = HorizontalGridLine.Instance.GetNearestTime(eventData.position.y);
        if (index > 0)
        {
            double last_time = cfg.checkPoints[index - 1].time;
            if (time <= last_time) return;
        }
        if (index < cfg.checkPoints.Count - 1)
        {
            double next_time = cfg.checkPoints[cfg.checkPoints.Count - 1].time;
            if (time >= next_time) return;
        }
        double new_center = HorizontalGridLine.Instance.GetPositionX(eventData);
        double width = cfg.checkPoints[index].position_r - cfg.checkPoints[index].position_l;
        cfg.checkPoints[index].time = time;
        cfg.checkPoints[index].position_l = new_center - width * 0.5f;
        cfg.checkPoints[index].position_r = new_center + width * 0.5f;
        this.OnPaint();
    }
}
