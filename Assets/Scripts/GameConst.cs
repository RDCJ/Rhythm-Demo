using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameConst
{
    public static float drop_speed = 800f;
    public static float judge_line_y = 180f;
    
    public static float perfect_interval = 0.04f;
    public static float good_interval = 0.08f;
    public static float active_interval = 0.18f;

    public static float total_score = 1000000;
    public static float basic_score = 900000;
    public static float combo_score = 100000;

    public static Dictionary<int, string> DifficultyIndex = new()
    {
        {0, "Easy"},
        {1, "Normal"},
        {2, "Hard"}
    };

    public static Dictionary<ScoreMgr.ScoreLevel, float> score_factory = new()
    {
        { ScoreMgr.ScoreLevel.perfect, 1},
        { ScoreMgr.ScoreLevel.good, 0.5f },
        { ScoreMgr.ScoreLevel.bad, 0 },
    };

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

}
