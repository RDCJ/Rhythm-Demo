using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    private static MainCanvas instance;
    public static MainCanvas Instance {  get { return instance; } }
    private MainCanvas() { }


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 120;
#else
        Application.targetFrameRate = 144;
#endif
    }
}
