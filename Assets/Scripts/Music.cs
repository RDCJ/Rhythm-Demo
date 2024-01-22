using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;
using Test;

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
        public Dictionary<string, List<NoteCfg>> composition;

        public MusicCfg()
        {
            composition = new Dictionary<string, List<NoteCfg>>();
        }

        public MusicCfg(int music_id)
        {
            this.music_id = music_id.ToString();
            music_name = MusicResMgr.MusicIndex2Name[music_id];
            composition = new Dictionary<string, List<NoteCfg>>();
        }

        public static MusicCfg GetCfg(string music_id)
        {
            string path = Application.persistentDataPath + "/" + FileConst.music_data_path + music_id;
            if (!path.EndsWith(".json"))
                path = path + ".json";
            if (File.Exists(path))
            {
                string js = File.ReadAllText(path);
                return JsonMapper.ToObject<MusicCfg>(js);
            }
            else
                return new MusicCfg(int.Parse(music_id));
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
            if (composition.ContainsKey(difficulty))
                return composition[difficulty];
            else
                return new List<NoteCfg>();
        }

        public void CompositionSerialize(ref List<NoteCfg> input, string difficulty)
        {
            List<NoteCfg> listDeep = new List<NoteCfg>();
            for (var i = 0; i < input.Count; i++)
                listDeep.Add(new NoteCfg(input[i]));

            listDeep.Sort((x, y) => x.FirstCheckPoint().time.CompareTo(y.FirstCheckPoint().time));

            if (composition == null)
                composition = new Dictionary<string, List<NoteCfg>>();
            composition[difficulty] = listDeep;
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
        public List<CheckPoint> checkPoints;
        public CheckPoint FirstCheckPoint()
        {
            if (checkPoints == null) return null;
            return checkPoints[0];
        }

        public CheckPoint LastCheckPoint()
        {
            if (checkPoints == null) return null;
            return checkPoints[checkPoints.Count - 1];
        }

        public double Duration()
        {
            if (checkPoints == null) return 0.0f;
            else if (checkPoints.Count <= 1) return 0.0f;
            else
                return LastCheckPoint().time - FirstCheckPoint().time;
        }

        public NoteCfg()
        {
            checkPoints = new List<CheckPoint>();
        }

        public NoteCfg(NoteCfg other)
        {
            this.note_type = other.note_type;
            this.checkPoints = new(other.checkPoints.Count);
            for (int i = 0; i < other.checkPoints.Count; i++)
            {
                this.AddCheckPoint(other.checkPoints[i].time, other.checkPoints[i].position_x);
            }
        }

        public void AddCheckPoint(double time, double position_x)
        {
            checkPoints.Add(new CheckPoint(time, position_x));
        }

        public JsonData ToJsonData()
        {
            JsonData rt = new JsonData();
            rt["note_type"] = note_type;
            rt["checkPoints"] = JsonMapper.ToJson(checkPoints);
            return rt;
        }
    }

    [Serializable]
    public class CheckPoint
    {
        public double time;
        public double position_x;
        public CheckPoint() { }
        public CheckPoint(double time, double position_x)
        {
            this.time = time;
            this.position_x = position_x;
        }

        public CheckPoint(CheckPoint other)
        {
            this.time = other.time;
            this.position_x = other.position_x;
        }
    }
}



