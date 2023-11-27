using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWindow : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = this.GetComponent<RectTransform>();
        Debug.Log(WindowSize());
    }
    
    public Vector2 WindowSize()
    {
        Vector2 size = rect.sizeDelta;
        return size;
/*        Vector3 scale = rect.localScale;
        return new Vector2(size.x * scale.x, size.y * scale.y);*/
    }
}
