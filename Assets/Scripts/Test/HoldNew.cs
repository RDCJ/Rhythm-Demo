using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AirFishLab.ScrollingList.ListBank;

public class HoldNew : MonoBehaviour
{
    private HoldPolygonRawImage icon;
    Music.NoteCfg noteCfg;
    RectTransform rectTransform;

    private void Awake()
    {
        icon = transform.Find("icon").GetComponent<HoldPolygonRawImage>();
        rectTransform = GetComponent<RectTransform>();
        noteCfg = new Music.NoteCfg();
        noteCfg.AddCheckPoint(1, 0.5);
        noteCfg.AddCheckPoint(2, 0.7);
        noteCfg.AddCheckPoint(3, 0.5);
        noteCfg.AddCheckPoint(4, 0.1);
    }

    // Start is called before the first frame update
    void Start()
    {
        icon.Cfg = noteCfg;

/*        foreach (var k in MusicResMgr.MusicIndex2Name.Keys)
        {
            Music.MusicCfg old_cfg = Music.MusicCfg.GetCfg(k.ToString());
            Test.MusicCfg new_cfg = Test.MusicCfg.OldVersionToNew(old_cfg);
            new_cfg.Save();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
