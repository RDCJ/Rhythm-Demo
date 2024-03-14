using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRaycaster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameTouchManager.Instance.AddListener(TouchPhase.Began, Raycast);
    }



    void Raycast(Touch touch_event)
    {
        var hits = Physics2D.RaycastAll(touch_event.position, Vector2.up);
        foreach (var hit in hits)
        {
            Debug.Log(hit.collider.name);
        }
    }
}
