using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchEventType
{
    Down,
    Up,
    Hold
}

public class TouchEvent
{
    public int finger_id;
    public Vector2 position;
    public float time;
    public TouchEventType type;

    public TouchEvent(int finger_id, Vector2 position, float time, TouchEventType type)
    {
        this.finger_id = finger_id;
        this.position = position;
        this.time = time;
        this.type = type;
    }
}

public delegate void TouchEventDelegate(Touch event_data);

public class GameTouchManager : MonoBehaviour
{
    /*    public bool PrintLog;
        bool is_finger_down = false;
        Vector2 down_position;
        Vector2 up_position;
        float up_time;
        float down_time;*/
    #region Singleton
    private GameTouchManager() { }
    private static GameTouchManager instance;
    public static GameTouchManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    private Dictionary<TouchPhase, TouchEventDelegate> event_handlers;

    /*    private void Start()
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
        }*/

    // Update is called once per frame
    public void Reset()
    {
        event_handlers = new Dictionary<TouchPhase, TouchEventDelegate>();
    }

    private void Awake()
    {
        instance = this;
        Reset();
    }

    void Update()
    {
        if (Input.touches.Length > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (event_handlers.ContainsKey(touch.phase))
                    event_handlers[touch.phase]?.Invoke(touch);
/*                touch.
                if (touch.phase == TouchPhase.Began)
                {
                    is_finger_down = true;
                    down_position = touch.position;
                    down_time = Time.time;
                    var new_event = new TouchEvent(touch.fingerId, touch.position, Time.time, TouchEventType.Down);
                    *//*                var new_event = new DownEvent(touch.fingerId, down_position, down_time);
                                    onDown?.Invoke(new_event);*//*
                    event_handlers[]
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (is_finger_down)
                    {
                        is_finger_down = false;
                        up_position = touch.position;
                        up_time = Time.time;
                        *//*                    var new_event = new SlideEvent(down_position, up_position, down_time, up_time);
                                            onSlide?.Invoke(new_event);*//*
                    }
                }
                TouchPhase.*/
            }
        }
    }

    public void AddListener(TouchPhase type, TouchEventDelegate listener)
    {
        if (!event_handlers.ContainsKey(type))
            event_handlers.Add(type, null);
        event_handlers[type] += listener;
    }

    public void RemoveListener(TouchPhase type, TouchEventDelegate listener)
    {
        if (!event_handlers.ContainsKey(type)) return;
        event_handlers[type] -= listener;
    }
}