using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MusicBackground : MonoBehaviour
{
    Image bg_img;
    Texture2D bg_tex;
    GameObject bg_video;
    RawImage bg_videoRawImage;
    public VideoPlayer bg_videoPlayer;

    private void Awake()
    {
        bg_img = transform.Find("ImgBG").GetComponent<Image>();
        bg_video = transform.Find("VideoBG").gameObject;
        bg_videoPlayer = transform.Find("VideoBG/VideoPlayer").GetComponent<VideoPlayer>();
        bg_videoRawImage = transform.Find("VideoBG/RawImage").GetComponent<RawImage>();
    }


    public IEnumerator Init(string music_file_name, Action callback=null)
    {
        if (MusicResMgr.BGIsVideo(music_file_name))
        {
            bg_videoPlayer.url = MusicResMgr.GetBGFilePath(music_file_name);
            bg_img.gameObject.SetActive(false);
            bg_video.SetActive(true);
        }
        else
        {
            bg_img.gameObject.SetActive(true);
            bg_video.SetActive(false);
            yield return StartCoroutine(
                MusicResMgr.GetBG(music_file_name, (Texture2D tex) =>
                {
                    if (bg_tex != null)
                        Destroy(bg_tex);
                    if (tex != null)
                    {
                        bg_tex = tex;
                        bg_img.sprite = Sprite.Create(bg_tex, new Rect(0, 0, bg_tex.width, bg_tex.height), new Vector2(0.5f, 0.5f));
                        bg_img.color = Color.white;
                    }
                    else
                    {
                        bg_img.sprite = null;
                        bg_img.color = Color.black;
                    }
                })
            );
        }
        callback?.Invoke();
    }

    public void Play()
    {
        if (bg_video.activeSelf)
            bg_videoPlayer.Play();
    }

    public void Pause()
    {
        if (bg_video.activeSelf)
            bg_videoPlayer.Pause();
    }

    public void Stop()
    {
        if (bg_video.activeSelf)
            bg_videoPlayer.Stop();
    }
}
