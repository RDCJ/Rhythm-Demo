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
    int lastFrame;

    private void Start()
    {
        if (GameConst.enable_test_mode)
        {
            txt = GetComponent<Text>();
            refresh_time = 0;
            lastFrame = Time.frameCount;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        refresh_time -= Time.deltaTime;
        if (refresh_time < 0)
        {
            int currentFrame = Time.frameCount;
            float fps = ((currentFrame - lastFrame) / refresh_cd);
            txt.text = $"{prefix}{(int)fps}";
            refresh_time = refresh_cd;
            lastFrame = currentFrame;
        }
    }
}
