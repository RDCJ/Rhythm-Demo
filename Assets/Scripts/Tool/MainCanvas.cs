using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    private static MainCanvas instance;
    public static MainCanvas Instance {  get { return instance; } }
    private MainCanvas() { }

    Canvas canvas;
    CanvasScaler canvasScaler;

    private void Awake()
    {
        instance = this;
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 120;
#else
        Application.targetFrameRate = 144;
#endif

        
        Debug.Log("scaleFactor:" + canvas.scaleFactor);
    }

    /// <summary>
    /// canvasËõ·ÅÏµÊý
    /// </summary>
    public float GetScaleFactor {  get { return canvas.scaleFactor; } }

    public static float GetScaleX { get => Screen.width / instance.canvasScaler.referenceResolution.x; }
    public static float GetScaleY { get => Screen.height / instance.canvasScaler.referenceResolution.y; }
}
