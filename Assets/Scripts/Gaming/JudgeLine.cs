using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    Image line_img;
    Image mask;

    Color[] colors = new Color[3] { Color.white, new(107f/255, 226f/255, 1, 1), new(228f/255, 228f/255, 135f/255, 1)};
    private void Awake()
    {
        instance = this;
        line_img = GetComponent<Image>();
        mask = transform.Find("mask").GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Util.ChangeV3(transform.position, 1, Screen.height * GameConst.judge_line_y);
        mask.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeColor(int index)
    {
        line_img.color = colors[index];
    }
}
