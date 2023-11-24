using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    Button play_btn;
    Button editor_btn;

    public GameObject game_mgr;
    public GameObject composition_editor;
    public GameObject music_select;

    public Reporter reporter;

    private void Awake()
    {
        play_btn = transform.Find("play_btn").GetComponent<Button>();
        editor_btn = transform.Find("editor_btn").GetComponent<Button>();
        play_btn.onClick.AddListener(() => {
            //Instantiate(game_mgr, transform.parent);
            Instantiate(music_select, transform.parent);
        });

        editor_btn.onClick.AddListener(() => {
            Instantiate(composition_editor, transform.parent);
        });
        CheckExtractResource();
    }

    // <summary>
    /// 释放资源
    /// </summary>
    public void CheckExtractResource()
    {
        bool isExists = Directory.Exists(Application.persistentDataPath + "/MusicsData");
        if (isExists)
        {
            Debug.Log("跳过资源解包");
            return;   //文件已经解压过了，自己可添加检查文件列表逻辑
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    IEnumerator OnExtractResource()
    {
        Debug.Log("资源解包开始");
        play_btn.interactable = false;
        editor_btn.interactable = false;
        string dataPath = Application.persistentDataPath;  //数据目录
        string resPath = AppContentPath; //游戏包资源目录
        if (Directory.Exists(dataPath))
            Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);


        string[] folders = new string[1] {"MusicsData" };
        foreach (var folder in folders)
        {
            string in_dir = System.IO.Path.Combine(AppContentPath, folder);
            string out_dir = System.IO.Path.Combine(Application.persistentDataPath, folder);
            if (!Directory.Exists(out_dir))
                Directory.CreateDirectory(out_dir);
            string[] files = Directory.GetFiles(in_dir);
            foreach (var in_file in files)
            {
                if (in_file.EndsWith(".meta")) continue;
                string file_name = System.IO.Path.GetFileName(in_file);
                string out_file = System.IO.Path.Combine(out_dir, file_name);
                if (Application.platform == RuntimePlatform.Android)
                {
                    WWW www = new WWW(in_file);
                    yield return www;

                    if (www.isDone)
                    {
                        File.WriteAllBytes(out_file, www.bytes);
                    }
                    yield return 0;
                }
                else
                {
                    if (File.Exists(out_file))
                    {
                        File.Delete(out_file);
                    }
                    File.Copy(in_file, out_file, true);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        play_btn.interactable = true;
        editor_btn.interactable = true;
        Debug.Log("资源解包结束");
    }


    public static void Ls(string path, string title = null)
    {
        if (title == null)
            Debug.Log("<<<<<< ls " + path + " >>>>>>");
        else
            Debug.Log("<<<<<< ls " + title + ": " + path + " >>>>>>");
        string[] files = Directory.GetFiles(path);

        foreach (string file in files)
        {
            Debug.Log(file);
        }

        // 获取文件夹中的文件夹路径
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            Debug.Log(directory);
        }
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath
    {
        get
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    path = Application.dataPath + "/" + "StreamingAssets" + "/";
                    break;
            }
            return path;
        }
    }
}
