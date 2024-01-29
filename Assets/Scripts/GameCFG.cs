using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="GameCFG", menuName = "ScriptableObject/GameCFG", order =0)]
public class GameCFG : ScriptableObject
{
    [Header("谱面编辑器的基础下落速度")]
    public float editor_drop_speed;
    [Header("正常游戏的基础下落速度")]
    public float drop_speed;
    [Header("判定线的位置")]
    public float judge_line_y;
    [Header("perfect判定区间")]
    public float perfect_interval;
    [Header("good判定区间")]
    public float good_interval;
    [Header("bad判定区间")]
    public float active_interval;
    [Header("游戏总分")]
    public float total_score;
    [Header("基础得分占比")]
    public float basic_score_percent;
}
