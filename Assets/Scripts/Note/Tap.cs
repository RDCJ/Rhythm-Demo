using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;

public class Tap : NoteBase, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (is_active)
        {
            Debug.Log("tap");
            Destroy(this.gameObject);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Tap;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
