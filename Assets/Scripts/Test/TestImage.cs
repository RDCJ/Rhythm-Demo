using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TestImage : MonoBehaviour, IEventListener
{
    public Image img;

    public void OnEnable()
    {
        if (TestEventMgr.Instance != null)
            TestEventMgr.Instance.AddListener((int)TestEventMgr.EventId.Fade, this);
        else
            Debug.Log("TestEventMgr.Instance == null");
    }

    public void OnDisable()
    {
        TestEventMgr.Instance.RemoveListener((int)TestEventMgr.EventId.Fade, this);
    }

    public void HandleEvent(int event_id, params object[] args)
    {
        if (event_id == (int)TestEventMgr.EventId.Fade)
        {
            float alpha = (float)args[0];
            float time = (float)args[1];
            img.GetComponent<CanvasGroup>().DOFade(alpha, time);
        }
    }
}
