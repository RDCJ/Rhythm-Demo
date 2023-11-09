using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioWaveForm : MonoBehaviour, IPointerDownHandler
{
    #region Singleton
    private AudioWaveForm() { }
    private static AudioWaveForm instance;
    public static AudioWaveForm Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    public float width_scale;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public RawImage _rawImage;
    Slider slider;
    private const int origin_width = 1500;
    private RectTransform scroll_content_rect;
    private RectTransform slide_area_rect;

    private void Awake()
    {
        slider = this.GetComponent<Slider>();
        scroll_content_rect = transform.parent.GetComponent<RectTransform>();
        slide_area_rect = transform.Find("Handle Slide Area").GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadAudio();
        _rawImage.texture = BakeAudioWaveform();
    }

    public void LoadAudio()
    {
        audioClip = audioSource.clip;
    }

    /// <summary>
    /// 将AudioClip上挂载的音频文件生成频谱到一张Texture2D上
    /// </summary>
    /// <returns></returns>
    public Texture2D BakeAudioWaveform()
    {
        Vector2 new_size = new Vector2(origin_width * width_scale, 200);
        _rawImage.GetComponent<RectTransform>().sizeDelta = new_size;
        scroll_content_rect.sizeDelta = new_size;
        slide_area_rect.sizeDelta = new_size;

        int resolution = (int)(20 / width_scale);	// 控制生成波浪线的高度
        int width = (int)(origin_width * width_scale);		// 这个是最后生成的Texture2D图片的宽度
        int height = 200;       // 这个是最后生成的Texture2D图片的高度

        resolution = audioClip.frequency / resolution;

        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        float[] waveForm = new float[(samples.Length / resolution)];

        float min = float.MaxValue;
        float max = -float.MaxValue;

        for (int i = 0; i < waveForm.Length; i++)
        {
            waveForm[i] = 0;

            for (int j = 0; j < resolution; j++)
            {
                waveForm[i] += Mathf.Abs(samples[(i * resolution) + j]);
            }

            min = Mathf.Min(min, waveForm[i]);
            max = Mathf.Max(max, waveForm[i]);
            //waveForm[i] /= resolution;
        }

        Color backgroundColor = Color.black;
        Color waveformColor = Color.green;
        Color[] blank = new Color[width * height];
        Texture2D texture = new Texture2D(width, height);

        for (int i = 0; i < blank.Length; ++i)
        {
            blank[i] = backgroundColor;
        }

        texture.SetPixels(blank, 0);

        float xScale = (float)width / (float)waveForm.Length;

        int tMid = (int)(height / 2.0f);
        float yScale = 1;

        if (max > tMid)
        {
            yScale = tMid / max;
        }

        for (int i = 0; i < waveForm.Length; ++i)
        {
            int x = (int)(i * xScale);
            int yOffset = (int)(waveForm[i] * yScale);
            int startY = tMid - yOffset;
            int endY = tMid + yOffset;

            for (int y = startY; y <= endY; ++y)
            {
                texture.SetPixel(x, y, waveformColor);
            }
        }

        texture.Apply();
        return texture;
    }

    private void Update()
    {
        if (audioSource.isPlaying)
            slider.value = this.GetCurrentAudioTimeNormalize;
    }

    public void WidthUp()
    {
        width_scale += 0.2f;
        _rawImage.texture = BakeAudioWaveform();
    }

    public void WidthDown()
    {
        width_scale -= 0.2f;
        _rawImage.texture = BakeAudioWaveform();
    }

    /// <summary>
    /// 从光标所处的位置开始播放音乐
    /// </summary>
    public void Play()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = slider.value * audioSource.clip.length;
            audioSource.Play();
        }
    }
    
    public void Pause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }

    /// <summary>
    /// 暂停音乐，将光标移到鼠标点击的位置
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        Pause();
        ((IPointerDownHandler)slider).OnPointerDown(eventData);
    }

    /// <summary>
    /// 获取音乐当前播放的时刻
    /// </summary>
    public float GetCurrentAudioTime
    {
        get
        {
            return audioSource.time;
        }
    }

    /// <summary>
    /// 获取归一化后的音乐当前播放的时刻
    /// </summary>
    public float GetCurrentAudioTimeNormalize
    {
        get
        {
            return audioSource.time / audioSource.clip.length;
        }
    }
}
