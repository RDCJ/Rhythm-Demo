using DG.Tweening.Plugins.Core.PathCore;
using LitJson;
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
    Button exit_btn;

    public GameObject composition_editor;
    public GameObject music_select;

    public Reporter reporter;

    private void Awake()
    {
        GameConst.gameCFG = Resources.Load<GameCFG>("GameCFG");
        play_btn = transform.Find("play_btn").GetComponent<Button>();
        editor_btn = transform.Find("editor_btn").GetComponent<Button>();
        exit_btn = transform.Find("exit_btn").GetComponent<Button>();
        play_btn.onClick.AddListener(() => {
            Instantiate(music_select, transform.parent);
        });

        editor_btn.onClick.AddListener(() => {
            CompositionEditor.Instance.Open();
        });

        exit_btn.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        CheckExtractResource();
    }

    private void Start()
    {
        GameMgr.Instance.Close();
    }

    // <summary>
    /// �ͷ���Դ
    /// </summary>
    public void CheckExtractResource()
    {
        StartCoroutine(
            OnExtractResource(
                () =>
                {
                    play_btn.interactable = false;
                    editor_btn.interactable = false;
                },
                () =>
                {
                    play_btn.interactable = true;
                    editor_btn.interactable = true;
                    MusicResMgr.RefreshPersistentDataPathMusicList();
                    PlayerData.Load();
                }
           )
            
        );    //�����ͷ�Э�� 
    }

    IEnumerator OnExtractResource(Action callback1=null, Action callback2=null)
    {
        Debug.Log("��Դ�����ʼ");
        callback1?.Invoke();
       
        string dataPath = Application.persistentDataPath;  //����Ŀ¼
        string resPath = AppContentPath; //��Ϸ����ԴĿ¼

        string in_dir = System.IO.Path.Combine(resPath, FileConst.music_data_path);
        string out_dir = System.IO.Path.Combine(dataPath, FileConst.music_data_path);
        if (!Directory.Exists(out_dir))
        {
            Directory.CreateDirectory(out_dir);
            Debug.Log("�½�Ŀ¼: " + out_dir);
        }

        WWW _www = new WWW(resPath + "music_list.json");
        yield return _www;
        Dictionary<string, string> tmp_music_list;
        if (_www.isDone && string.IsNullOrEmpty(_www.error))
        {
            tmp_music_list = JsonMapper.ToObject<Dictionary<string, string>>(_www.text);
        }
        else
        {
            Debug.Log("music_list.json ����ʧ��");
            yield break;
        }

        foreach (var kv in tmp_music_list)
        {
            string music_name = kv.Key;
            string extension = kv.Value;
            string in_music_folder = System.IO.Path.Combine(in_dir, music_name);
            string out_music_folder = System.IO.Path.Combine(out_dir, music_name);
            if (!Directory.Exists(out_music_folder))
            {
                Directory.CreateDirectory(out_music_folder);
                Debug.Log("�½�Ŀ¼: " + out_music_folder);
            }
            else
            {
                Debug.Log("��Դ�Ѵ���, ���Ա��θ���: " + out_music_folder);
                continue;
            }
            List<string> files = new List<string>
            {
                music_name + extension,
                "music_cfg.json"
            };

            #region �����ļ�
            foreach (var file in files)
            {
                Debug.Log("��ʼ�����ļ�: " + file);
                if (file.EndsWith(".meta")) continue;
                string in_file_path = System.IO.Path.Combine(in_music_folder, file);
                string out_file_path = System.IO.Path.Combine(out_music_folder, file);
                if (Application.platform == RuntimePlatform.Android)
                {
                    WWW www = new WWW(in_file_path);
                    yield return www;

                    if (www.isDone && string.IsNullOrEmpty(www.error))
                    {
                        File.WriteAllBytes(out_file_path, www.bytes);
                        Debug.Log("�����ļ��ɹ�: " + out_file_path);
                    }
                    else
                    {
                        Debug.Log("�����ļ�ʧ��: " + out_file_path);
                    }
                    yield return 0;
                }
                else
                {
                    if (File.Exists(out_file_path))
                    {
                        File.Delete(out_file_path);
                    }
                    if (File.Exists(in_file_path))
                        File.Copy(in_file_path, out_file_path, true);
                }
                yield return new WaitForEndOfFrame();
            }
            #endregion

        }

        callback2?.Invoke();
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
