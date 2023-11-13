using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Music;
using LitJson;

public class TestPanel : MonoBehaviour
{
    public MusicCfg music_cfg;
    public List<NoteCfg> composition;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        string music_id = "1";
        string level = "hard";
        music_cfg = MusicCfg.GetCfgFromEditor(music_id);
        if (!music_cfg.composition.Keys.Contains(level))
        {
            Debug.Log("Level: " + level + " is invalid");
        }
        else
        {
            music_cfg.GetCompostion(ref composition, level);
        }
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
