using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorEventMgr : EventMgr
{
    public enum EventID
    {
        MusicPlay,
        MusicPause,
        WaveFormWidthUp,
        WaveFormWidthDown,
        WaveFormSliderValueChange
    }

    #region Singleton
    private EditorEventMgr() { }
    private static EditorEventMgr instance;
    public static EditorEventMgr Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion
}
