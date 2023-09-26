using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Hold : Note, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    bool is_holding;
    float hold_max_duration;
    float start_time;
    float end_time;

    public void OnPointerDown(PointerEventData eventData)
    {
        is_holding = true;
        start_time = Time.time;
        Debug.Log("Hold start " + start_time);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            end_time = Time.time;
            Debug.Log("Hold end " + end_time);
            Destroy(this.gameObject);
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            end_time = Time.time;
            Debug.Log("Hold end " + end_time);
            Destroy(this.gameObject);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        is_holding = false;
        Resize();
        Drop();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    private void Resize()
    {
        float x = rectTransform.sizeDelta.x;
        float y = hold_max_duration * 100;
        rectTransform.sizeDelta = new Vector2(x, y);
    }


}
