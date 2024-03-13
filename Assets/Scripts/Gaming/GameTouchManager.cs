using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DownEvent
{
    public Vector2 down_position;
    public float down_time;
    public DownEvent(Vector2 down_position, float down_time)
    {
        this.down_position = down_position;
        this.down_time = down_time;
    }
}

public class SlideEvent
{
    public Vector2 down_position;
    public Vector2 up_position;
    public float down_time;
    public float up_time;
    public SlideEvent(Vector2 down_position, Vector2 up_position, float down_time, float up_time)
    {
        this.down_position = down_position;
        this.up_position = up_position;
        this.down_time = down_time;
        this.up_time = up_time;
    }

    public float duration => up_time - down_time;
}

public delegate void DownEventDelegate(DownEvent event_data);
public delegate void SlideEventDelegate(SlideEvent event_data);

public class GameTouchManager : MonoBehaviour
{
    public bool PrintLog;
    bool is_finger_down = false;
    Vector2 down_position;
    Vector2 up_position;
    float up_time;
    float down_time;

    private static DownEventDelegate onDown;
    private static SlideEventDelegate onSlide;

    private void Start()
    {
        AddListener((SlideEvent event_data) =>
        {
            if (PrintLog)
                Debug.Log("OnSlide: " + event_data.down_position + "->" + event_data.up_position);
        });

        AddListener((DownEvent event_data) =>
        {
            if (PrintLog)
                Debug.Log("OnDown: " + event_data.down_position);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                is_finger_down = true;
                down_position = touch.position;
                down_time = Time.time;
                var new_event = new DownEvent(down_position, down_time);
                onDown?.Invoke(new_event);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (is_finger_down)
                {
                    is_finger_down = false;
                    up_position = touch.position;
                    up_time = Time.time;
                    var new_event = new SlideEvent(down_position, up_position, down_time, up_time);
                    onSlide?.Invoke(new_event);
                }
            }
        }
    }

    public static void AddListener(SlideEventDelegate listener)
    {
        onSlide += listener;
    }

    public static void RemoveListener(SlideEventDelegate listener)
    {
        onSlide -= listener;
    }

    public static void AddListener(DownEventDelegate listener)
    {
        onDown += listener;
    }

    public static void RemoveListener(DownEventDelegate listener)
    {
        onDown -= listener;
    }


}