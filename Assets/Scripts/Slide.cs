using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Slide : Note, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum SlideDirection
    {
        Left,
        Right
    }
    public SlideDirection direciton;

    private bool is_down;
    private Vector2 down_position;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (is_active)
        {
            is_down = true;
            down_position = eventData.position;
            Debug.Log("slide down " + down_position);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (is_down)
        {
            Vector2 move_position = eventData.position;
            Vector2 slide_direction = move_position - down_position;

            Destroy(this.gameObject);
            if ((slide_direction.x < 0) == (direciton == SlideDirection.Left))
                Debug.Log("sliding right " + eventData.position);
            else
                Debug.Log("sliding wrong" + eventData.position);
        }
            
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        is_down = false;
        Debug.Log("slide up");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        is_down = false;
        Drop();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
