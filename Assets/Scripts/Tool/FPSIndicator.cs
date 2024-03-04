using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSIndicator : MonoBehaviour
{
    Text txt;

    string prefix = "FPS: ";
    float refresh_cd = 0.1f;
    float refresh_time;
    private void Awake()
    {
#if UNITY_EDITOR
        txt = GetComponent<Text>();
        refresh_time = 0;
#else
        Destroy(this.gameObject);
#endif
    }
    // Update is called once per frame
    void Update()
    {
        refresh_time -= Time.deltaTime;
        if (refresh_time < 0)
        {
            float fps = (1f / Time.deltaTime);
            txt.text = prefix + ((int)fps).ToString();
            refresh_time = refresh_cd;
        }
    }
}
