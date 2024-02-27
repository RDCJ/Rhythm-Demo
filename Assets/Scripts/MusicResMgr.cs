using LitJson;
using Music;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using UnityEngine.Networking;

public class MusicFileExtension
{
    public string audio_extension;
    public string bg_extension;
}

public class MusicResMgr
{
    public static Dictionary<string, MusicFileExtension> music_list;

    public static void RefreshPersistentDataPathMusicList()
    {
        string res_dir = Path.Combine(Application.persistentDataPath, FileConst.music_data_path);
        music_list = ScanMusicData(res_dir);
    }

    public static IEnumerator GetMusic(string music_file_name, Action<AudioClip> callback=null)
    {
        if (music_list.ContainsKey(music_file_name))
        {
            string music_file_path = Path.Combine(Application.persistentDataPath, FileConst.music_data_path, music_file_name);
            music_file_path = "file://" + Path.Combine(music_file_path, music_file_name + music_list[music_file_name].audio_extension);

            UnityWebRequest request = null;
            if (music_list[music_file_name].audio_extension.Contains("mp3"))
                request = UnityWebRequestMultimedia.GetAudioClip(music_file_path, AudioType.MPEG);
            else if (music_list[music_file_name].audio_extension.Contains("wav"))
                request = UnityWebRequestMultimedia.GetAudioClip(music_file_path, AudioType.WAV);
            yield return request.SendWebRequest();

            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            if (audioClip == null)
                Debug.Log("音乐加载失败: " + music_file_name);
            else
                callback?.Invoke(audioClip);
        }
        else
        {
            Debug.Log("音乐未找到: " + music_file_name);
            yield break;
        }
    }

    public static IEnumerator GetBG(string music_file_name, Action<Texture2D> callback = null)
    {
        if (music_list.ContainsKey(music_file_name))
        {
            string bg_file_path = GetBGFilePath(music_file_name);

            UnityWebRequest request = null;
            request = UnityWebRequestTexture.GetTexture(bg_file_path);
            yield return request.SendWebRequest();


            Texture2D texture = null;
            if (request.responseCode == 200)
            {
                texture = DownloadHandlerTexture.GetContent(request);
            }
            if (texture == null)
                Debug.Log("背景加载失败: " + music_file_name);
            callback?.Invoke(texture);
        }
        else
        {
            Debug.Log("音乐未找到: " + music_file_name);
            yield break;
        }
    }

    public static string GetBGFilePath(string music_file_name)
    {
        string bg_file_path = Path.Combine(Application.persistentDataPath, FileConst.music_data_path, music_file_name);
        bg_file_path = "file://" + Path.Combine(bg_file_path, "BG" + music_list[music_file_name].bg_extension);
        return bg_file_path;
    }

    public static bool BGIsVideo(string music_file_name)
    {
        if (music_list.ContainsKey(music_file_name))
        {
            if (music_list[music_file_name].bg_extension != null)
            {
                return music_list[music_file_name].bg_extension.Contains("mp4");
            }
            else
                return false;
        }
        else
        {
            Debug.Log("音乐未找到: " + music_file_name);
            return false;
        }
    }

    public static MusicCfg GetCfg(string music_file_name)
    {
        string path = Path.Combine(Application.persistentDataPath, FileConst.music_data_path, music_file_name);
        path = Path.Combine(path, FileConst.music_cfg_file_name + ".json");
        if (File.Exists(path))
        {
            string js = File.ReadAllText(path);
            return JsonMapper.ToObject<MusicCfg>(js);
        }
        else
            return new MusicCfg(music_file_name);
    }

    public static Dictionary<string, MusicFileExtension> ScanMusicData(string music_data_dir)
    {
        Dictionary<string, MusicFileExtension> music_list = new();
        // 遍历资源文件夹，每首乐曲放在单独的一个文件夹中
        foreach (string folder in Directory.GetDirectories(music_data_dir))
        {
            bool flag = false;
            string folder_name = Path.GetFileName(folder);
            music_list[folder_name] = new MusicFileExtension();
            foreach (string file in Directory.GetFiles(folder))
            {
                string file_name = Path.GetFileNameWithoutExtension(file);
                // 识别与文件夹同名的音乐文件
                if (file_name == folder_name)
                {
                    flag = true;
                    music_list[folder_name].audio_extension = Path.GetExtension(file);
                }
                else if (file_name == "BG")
                {
                    music_list[folder_name].bg_extension = Path.GetExtension(file);
                }
            }
            if (!flag)
            {
                Debug.Log("未找到与文件夹同名的音乐文件: " + folder_name);
                music_list.Remove(folder_name);
            }
        }
        return music_list;
    }

#if UNITY_EDITOR
    [MenuItem("Tools/生成音乐列表")]
    public static void StreamingAssetsMusicList()
    {
        int k = 0;
        string data_dir = Path.Combine(Application.dataPath, "StreamingAssets", FileConst.music_data_path);
        string output_path = Application.dataPath + "/StreamingAssets" + "/music_list.json";

        var music_list = ScanMusicData(data_dir);

        Debug.Log("从StreamingAssets生成音乐列表: " + output_path);

        string jsonStr = JsonMapper.ToJson(music_list);

        File.WriteAllText(output_path, jsonStr);
    }
#endif
}
