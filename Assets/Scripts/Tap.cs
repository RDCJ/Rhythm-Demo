using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Tap : Note, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (is_active)
        {
            Debug.Log("tap");
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        is_active = false;
        Drop();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
