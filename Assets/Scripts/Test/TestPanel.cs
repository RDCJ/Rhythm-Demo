using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Music;
using LitJson;
using UnityEngine.UI;

public class TestPanel : MonoBehaviour
{
    public Button btn;
    public Image img;

    private void Awake()
    {
        btn.onClick.AddListener(() => {
            TestEventMgr.Instance.Dispatch((int)TestEventMgr.EventId.Fade, 0.5f, 1.0f);
        });
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TestClass
{
    public int x;
    public int y;
    public JsonData z;
}
