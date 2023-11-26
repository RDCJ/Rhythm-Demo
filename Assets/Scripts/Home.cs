using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
    /// �ͷ���Դ
    /// </summary>
    public void CheckExtractResource()
    {
        bool isExists = Directory.Exists(Application.persistentDataPath + "/MusicsData");
        if (isExists)
        {
            Debug.Log("������Դ���");
            return;   //�ļ��Ѿ���ѹ���ˣ��Լ�����Ӽ���ļ��б��߼�
        }
        StartCoroutine(OnExtractResource());    //�����ͷ�Э�� 
    }

    IEnumerator OnExtractResource()
    {
        Debug.Log("��Դ�����ʼ");
        play_btn.interactable = false;
        editor_btn.interactable = false;
        string dataPath = Application.persistentDataPath;  //����Ŀ¼
        string resPath = AppContentPath; //��Ϸ����ԴĿ¼
        if (Directory.Exists(dataPath))
        {
            Directory.Delete(dataPath, true);
            Debug.Log("ɾ��Ŀ¼: " + dataPath);
        }

        Debug.Log("�½�Ŀ¼: " + dataPath);
        Directory.CreateDirectory(dataPath);


        string[] folders = new string[1] {"MusicsData" };
        foreach (var folder in folders)
        {
            string in_dir = System.IO.Path.Combine(resPath, folder);
            string out_dir = System.IO.Path.Combine(dataPath, folder);
            if (!Directory.Exists(out_dir))
            {
                Directory.CreateDirectory(out_dir);
                Debug.Log("�½�Ŀ¼: " + out_dir);
            }

            List<string> files = new List<string>();
            foreach (var key in MusicResMgr.MusicIndex2Name.Keys)
                files.Add(in_dir + "/" + key.ToString() + ".json");

            foreach (var in_file in files)
            {
                Debug.Log("��ʼ�����ļ�:" + in_file);
                if (in_file.EndsWith(".meta")) continue;
                string file_name = System.IO.Path.GetFileName(in_file);
                string out_file = System.IO.Path.Combine(out_dir, file_name);
                if (Application.platform == RuntimePlatform.Android)
                {
                    WWW www = new WWW(in_file);
                    yield return www;

                    if (www.isDone && string.IsNullOrEmpty(www.error))
                    {
                        File.WriteAllBytes(out_file, www.bytes);
                        Debug.Log("�����ļ��ɹ�:" + out_file);
                    }
                    yield return 0;
                }
                else
                {
                    if (File.Exists(out_file))
                    {
                        File.Delete(out_file);
                    }
                    if (File.Exists(in_file))
                        File.Copy(in_file, out_file, true);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        play_btn.interactable = true;
        editor_btn.interactable = true;
        Debug.Log("��Դ�������");
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

        // ��ȡ�ļ����е��ļ���·��
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            Debug.Log(directory);
        }
    }

    /// <summary>
    /// Ӧ�ó�������·��
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
