using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;

namespace Music
{
    [Serializable]
    public class MusicCfg
    {
        public string music_id;
        public string music_name;
        public string author;
        public double time;
        public JsonData composition;

        public static MusicCfg GetCfg(string file_name)
        {
            string path = FileConst.music_data_path + file_name;
            string js = File.ReadAllText(path);
            GameMgr.Instance.text.text = js;
            return JsonMapper.ToObject<MusicCfg>(js);
        }

        public static string GetCfgNameByID(string id)
        {
            string[] files = Directory.GetFiles(FileConst.music_data_path);
            // 获取当前文件夹下所有文件的路径
            foreach (string file in files)
            {
                string file_name = Path.GetFileName(file);
                // 检查文件名是否具有特定前缀
                if (file_name.StartsWith(id) && !file_name.EndsWith(".meta"))
                {
                    return file_name;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class NoteCfg
    {
        public int note_type;
        public double time;
        public double position_x;
        public double duration;
    }
}



