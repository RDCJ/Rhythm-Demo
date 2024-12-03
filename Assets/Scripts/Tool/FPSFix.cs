using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSFix : MonoBehaviour
{
    public int PCMaxFPS;
    public int MobileMaxFPS;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = MobileMaxFPS;
#else
        Application.targetFrameRate = PCMaxFPS;
#endif
        Destroy(this);
    }
}
