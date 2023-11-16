using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeLine : MonoBehaviour
{
    #region Singleton
    private JudgeLine() { }
    private static JudgeLine instance;
    public static JudgeLine Instance
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
        float x = transform.position.x;
        float y = GameConst.judge_line_y;
        transform.position = new Vector3(x, y, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
