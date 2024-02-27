using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
   


    public class Test : MonoBehaviour
    {
        public Image img;

        // Start is called before the first frame update
        void Start()
        {
            MusicResMgr.RefreshPersistentDataPathMusicList();
            StartCoroutine(
                MusicResMgr.GetBG("Noël", (Texture2D TEX) =>{
                img.sprite = Sprite.Create(TEX, new Rect(0, 0, TEX.width, TEX.height), new Vector2(0.5f, 0.5f));
                })
            );
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}

