using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    private static MainCanvas instance;
    public static MainCanvas Instance {  get { return instance; } }
    private MainCanvas() { }

    CanvasScaler canvasScaler;

    private void Awake()
    {
        instance = this;
        canvasScaler = GetComponent<CanvasScaler>();
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 120;
#else
        Application.targetFrameRate = 144;
#endif

        
        Debug.Log("scaleFactorX :" + GetScaleX + " scaleFactorY: " + GetScaleY);
    }

    /// <summary>
    /// canvasËõ·ÅÏµÊý
    /// </summary>
    public static float GetScaleX { get => Screen.width / instance.canvasScaler.referenceResolution.x; }
    public static float GetScaleY { get => Screen.height / instance.canvasScaler.referenceResolution.y; }
}
