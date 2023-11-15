using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWindow : MonoBehaviour
{
    private GameWindow() { }
    private static GameWindow instance;
    public static GameWindow Instance
    {
        get
        {
            return instance;
        }
    }

    RectTransform rect;

    private void Awake()
    {
        instance = this;
        rect = this.GetComponent<RectTransform>();
    }
    
    public Vector2 WindowSize()
    {
        Vector2 size = rect.sizeDelta;
        return size;
/*        Vector3 scale = rect.localScale;
        return new Vector2(size.x * scale.x, size.y * scale.y);*/
    }
}
