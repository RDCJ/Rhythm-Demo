using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameConst
{
    public static float editor_drop_speed = 300;
    public static float drop_speed = 1200;
    public static float judge_line_y = 0.167f;
    
    public static float perfect_interval = 0.04f;
    public static float good_interval = 0.08f;
    public static float active_interval = 0.18f;

    public static float total_score = 1000000;
    public static float basic_score_percent = 0.9f;
    public static float basic_score 
    { 
        get => total_score * basic_score_percent;
    } 
    public static float combo_score
    {
        get => total_score * (1- basic_score_percent);
    }

    public static Dictionary<int, string> DifficultyIndex = new()
    {
        {0, "Easy"},
        {1, "Normal"},
        {2, "Hard"}
    };

    /// <summary>
    /// 相对于perfect的得分系数，用于计算基础得分
    /// </summary>
    public static Dictionary<ScoreMgr.ScoreLevel, float> score_factory = new()
    {
        { ScoreMgr.ScoreLevel.perfect, 1},
        { ScoreMgr.ScoreLevel.good, 0.5f },
        { ScoreMgr.ScoreLevel.bad, 0 },
    };

    /// <summary>
    /// 准确率权重
    /// </summary>
    public static Dictionary<ScoreMgr.ScoreLevel, int> acc_weight = new()
    {
        { ScoreMgr.ScoreLevel.perfect, 10 },
        { ScoreMgr.ScoreLevel.good, 5 },
        { ScoreMgr.ScoreLevel.bad, 0 },
    };
}

public class FileConst
{
    public static string resources_path = Application.dataPath + "/Resources/";
    public static string music_data_path = "MusicsData/";

    public static string tap_prefab_path = "Prefabs/Note/TapNote";
    public static string leftslide_prefab_path = "Prefabs/Note/SlideLeftNote";
    public static string rightslide_prefab_path = "Prefabs/Note/SlideRightNote";
    public static string hold_prefab_path = "Prefabs/Note/HoldNote";
    public static string catch_prefab_path = "Prefabs/Note/CatchNote";

    public static string perfect_effect_prefab_path = "Prefabs/Effect/Perfect";
    public static string good_effect_prefab_path = "Prefabs/Effect/Good";
    public static string bad_effect_prefab_path = "Prefabs/Effect/Bad";

}
