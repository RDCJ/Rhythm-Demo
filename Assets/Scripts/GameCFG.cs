using UnityEngine;

[CreateAssetMenu(fileName ="GameCFG", menuName = "ScriptableObject/GameCFG", order =0)]
public class GameCFG : ScriptableObject
{
    [Header("谱面编辑器的基础下落速度")]
    public float editor_drop_speed;
    [Header("正常游戏的基础下落速度")]
    public float drop_speed;
    [Header("判定线的位置")]
    public float judge_line_y;
    [Header("perfect判定区间(s)")]
    public float perfect_interval;
    [Header("good判定区间(s)")]
    public float good_interval;
    [Header("可判定区间(s)")]
    public float active_interval;
    [Header("游戏总分")]
    public float total_score;
    [Header("基础得分占比")]
    public float basic_score_percent;
    [Header("Hold音符判定宽度延长")]
    public float hold_touch_area_width_extend;

    [Header("显示判定区域 [编辑器中有效]")]
    public bool note_show_touch_area;
    [Header("hold_note下落 [编辑器中有效]")]
    public bool hold_note_drop;
    [Header("其他note下落 [编辑器中有效]")]
    public bool note_drop;
    [Header("启用测试模式")]
    public bool enable_test_mode;
    

}
