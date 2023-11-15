using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventMgr : EventMgr
{
    public enum EventId
    {
        Fade,
        PlaySmile
    }

    #region Singleton
    private TestEventMgr() { }
    private static TestEventMgr instance;
    public static TestEventMgr Instance
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
