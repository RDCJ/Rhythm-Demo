using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameConst
{
    public static GameCFG gameCFG;
    /// <summary>
    /// ����༭���Ļ��������ٶ�
    /// </summary>
    public static float editor_drop_speed => gameCFG.editor_drop_speed;
    /// <summary>
    /// ������Ϸ�Ļ��������ٶ�
    /// </summary>
    public static float drop_speed => gameCFG.drop_speed;
    /// <summary>
    /// �ж��ߵ�λ��
    /// </summary>
    public static float judge_line_y => gameCFG.judge_line_y;
    /// <summary>
    /// perfect�ж�����
    /// </summary>
    public static float perfect_interval => gameCFG.perfect_interval;
    /// <summary>
    /// good�ж�����
    /// </summary>
    public static float good_interval => gameCFG.good_interval;
    /// <summary>
    /// ���ж�����
    /// </summary>
    public static float active_interval => gameCFG.active_interval;
    /// <summary>
    /// ��Ϸ�ܷ�
    /// </summary>
    public static float total_score => gameCFG.total_score;
    /// <summary>
    /// �����÷�ռ��
    /// </summary>
    public static float basic_score_percent => gameCFG.basic_score_percent;
    /// <summary>
    /// ��ʾ�ж����� [�༭������Ч]
    /// </summary>
    public static bool note_show_touch_area=> gameCFG.note_show_touch_area;

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
    /// �����perfect�ĵ÷�ϵ�������ڼ�������÷�
    /// </summary>
    public static Dictionary<ScoreMgr.ScoreLevel, float> score_factory = new()
    {
        { ScoreMgr.ScoreLevel.perfect, 1},
        { ScoreMgr.ScoreLevel.good, 0.5f },
        { ScoreMgr.ScoreLevel.bad, 0 },
    };

    /// <summary>
    /// ׼ȷ��Ȩ��
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
    public static string music_data_path = "MusicsData";

    public static string tap_prefab_path = "Prefabs/Note/TapNote";
    public static string leftslide_prefab_path = "Prefabs/Note/SlideLeftNote";
    public static string rightslide_prefab_path = "Prefabs/Note/SlideRightNote";
    public static string hold_prefab_path = "Prefabs/Note/HoldNote";
    public static string catch_prefab_path = "Prefabs/Note/CatchNote";

    public static string perfect_effect_prefab_path = "Prefabs/Effect/Perfect";
    public static string good_effect_prefab_path = "Prefabs/Effect/Good";
    public static string bad_effect_prefab_path = "Prefabs/Effect/Bad";

    public static string music_cfg_file_name = "music_cfg";

}
