using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNew : MonoBehaviour
{
    private HoldPolygonRawImage icon;
    HoldPolygonRawImage.NoteCfg noteCfg;
    RectTransform rectTransform;

    private void Awake()
    {
        icon = transform.Find("icon").GetComponent<HoldPolygonRawImage>();
        rectTransform = GetComponent<RectTransform>();
        noteCfg = new HoldPolygonRawImage.NoteCfg();
        noteCfg.AddCheckPoint(1, 0.5);
        noteCfg.AddCheckPoint(2, 0.7);
        noteCfg.AddCheckPoint(3, 0.5);
        noteCfg.AddCheckPoint(4, 0.1);
    }

    // Start is called before the first frame update
    void Start()
    {
        icon.Cfg = noteCfg;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
