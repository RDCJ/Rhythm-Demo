using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMgr : MonoBehaviour
{
    public enum ScoreLevel
    {
        perfect,
        good,
        bad
    }

    #region Singleton
    private ScoreMgr() { }
    private static ScoreMgr instance;
    public static ScoreMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
