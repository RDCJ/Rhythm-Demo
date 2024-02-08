using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckPointDrag : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public enum TYPE
    {
        Left,
        Right,
        Center
    }

    [SerializeField]
    EditorHoldPainter hold;

    public RectTransform rectTransform;

    /// <summary>
    /// 对应cfg.checkPoint中的index
    /// </summary>
    public int index;

    public TYPE type;

    float click_interval = 0.3f;
    float last_click_time;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.Left:
                hold.OnCheckPointDragLeft(eventData, index);
                break;
            case TYPE.Right:
                hold.OnCheckPointDragRight(eventData, index);
                break;
            case TYPE.Center:
                hold.OnCheckPointDragCenter(eventData, index);
                break;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float time_span = Time.time - last_click_time;
        if (time_span <= click_interval)
        {
            if (index == hold.cfg.checkPoints.Count - 1)
            {
                hold.AddCheckPoint();
                last_click_time = 0;
            }
        }
        last_click_time = Time.time;
    }
}
