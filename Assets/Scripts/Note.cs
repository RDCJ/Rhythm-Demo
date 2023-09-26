using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Note : MonoBehaviour
{
    [HideInInspector] public bool is_active;
    public float start_x;

    protected RectTransform rectTransform;
    protected RectTransform judgeTigger_rect;
    protected virtual void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        judgeTigger_rect = transform.Find("JudgeTrigger").GetComponent<RectTransform>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }
    public void Activate()
    {
        is_active = true;
        OnActive();
    }
    protected virtual void OnActive()
    {

    }

    protected void ResetPosition()
    {
        rectTransform.anchoredPosition = new Vector2(start_x, (Screen.height + rectTransform.sizeDelta.y) / 2);
    }

    protected void Drop()
    {
        ResetPosition();
        transform.DOMoveY((Screen.height + rectTransform.sizeDelta.y) / 2, 10);
    }

    public virtual void Miss() { }
}
