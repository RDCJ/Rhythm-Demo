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
        public double prepare_time;
        public double time_offset;
        public int BPM;
        public JsonData composition;

        public MusicCfg() {
            composition = new JsonData();
        }

        public MusicCfg(int music_id)
        {
            this.music_id = music_id.ToString();
            music_name = MusicResMgr.MusicIndex2Name[music_id];
            composition = new JsonData();
        }

        public static MusicCfg GetCfg(string file_name)
        {
            string path = FileConst.music_data_path + file_name;
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            string js = textAsset.text;
            return JsonMapper.ToObject<MusicCfg>(js);
        }

        public static MusicCfg GetCfgFromEditor(string file_name)
        {
            string path = Application.persistentDataPath + "/" + FileConst.music_data_path + file_name;
            if (!path.EndsWith(".json"))
                path = path + ".json";
            if (File.Exists(path))
            {
                string js = File.ReadAllText(path);
                return JsonMapper.ToObject<MusicCfg>(js);
            }
            else
                return new MusicCfg(int.Parse(file_name));
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

        public List<NoteCfg> GetComposition(string difficulty)
        {
            List<NoteCfg> output = new List<NoteCfg>();
            if (composition.Keys.Contains(difficulty) && composition[difficulty] != null && composition[difficulty].GetJsonType() == JsonType.Array)
            {
                for (int i = 0; i < composition[difficulty].Count; i++)
                    output.Add(JsonMapper.ToObject<NoteCfg>(composition[difficulty][i].ToJson()));
            }
            return output;
        }

        public void CompositionSerialize(ref List<NoteCfg> input, string difficulty)
        {
            List<NoteCfg> listDeep = new List<NoteCfg>();
            for (var i = 0; i < input.Count; i++)
                listDeep.Add(new NoteCfg(input[i]));

            listDeep.Sort((x, y) =>x.time.CompareTo(y.time));

            if (composition == null)
                composition = new JsonData();
            composition[difficulty] = new JsonData();
            composition[difficulty].SetJsonType(JsonType.Array);
            foreach (var note_cfg in listDeep)
            {
                composition[difficulty].Add(note_cfg.ToJsonData());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            string folder_path = Path.Combine(Application.persistentDataPath, FileConst.music_data_path);
            if (!Directory.Exists(folder_path))
            {
                //创建了一个路径为 path 文件夹的实例对象
                DirectoryInfo directoryInfo = new DirectoryInfo(folder_path);
                // 创建目录
                directoryInfo.Create();
            }
            string filePath = Path.Combine(folder_path, music_id + ".json");
            string json_txt = JsonMapper.ToJson(this);
            File.WriteAllText(filePath, json_txt);
        }
    }

    [Serializable]
    public class NoteCfg
    {
        public int note_type;
        public double time;
        public double position_x;
        public double duration;

        public NoteCfg()
        {

        }

        public NoteCfg(NoteCfg other)
        {
            note_type = other.note_type;
            time = other.time;
            position_x = other.position_x;
            duration = other.duration;
        }

        public JsonData ToJsonData() 
        {
            JsonData rt = new JsonData();
            rt["note_type"] = note_type;
            rt["time"] = time;
            rt["position_x"] = position_x;
            rt["duration"] = duration;
            return rt;
        }
    }
}



