using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    Button restart_btn;
    private void Awake()
    {
        restart_btn = transform.Find("Restart").GetComponent<Button>();
        restart_btn.onClick.AddListener(() => {
            SceneManager.LoadScene("SampleScene");
        });
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 60;
#else
        Application.targetFrameRate = -1;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
