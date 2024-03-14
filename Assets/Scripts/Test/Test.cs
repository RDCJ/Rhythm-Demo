using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Test
{
    public class Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GameTouchManager.Instance.AddListener(TouchPhase.Began, (Touch touch) =>{
                Debug.Log($"[{this.gameObject.name}] Touch.Began " + touch.position);
            
            });

            GameTouchManager.Instance.AddListener(TouchPhase.Ended, (Touch touch) =>
            {
                Debug.Log($"[{this.gameObject.name}] Touch.Ended " + touch.position);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}

