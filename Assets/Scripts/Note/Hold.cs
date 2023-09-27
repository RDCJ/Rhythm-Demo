using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using Music;

public class Hold : NoteBase, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    bool is_holding;
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
        type = NoteType.Hold;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Init(NoteCfg _cfg)
    {
        base.Init(_cfg);
        is_holding = false;
        Resize();
    }

    private void Resize()
    {
        float x = rectTransform.sizeDelta.x;
        float y = (float)cfg.duration * GameConst.drop_speed;
        rectTransform.sizeDelta = new Vector2(x, y);
    }


}
