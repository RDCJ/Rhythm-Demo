using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="GameCFG", menuName = "ScriptableObject/GameCFG", order =0)]
public class GameCFG : ScriptableObject
{
    [Header("����༭���Ļ��������ٶ�")]
    public float editor_drop_speed;
    [Header("������Ϸ�Ļ��������ٶ�")]
    public float drop_speed;
    [Header("�ж��ߵ�λ��")]
    public float judge_line_y;
    [Header("perfect�ж�����(s)")]
    public float perfect_interval;
    [Header("good�ж�����(s)")]
    public float good_interval;
    [Header("���ж�����(s)")]
    public float active_interval;
    [Header("��Ϸ�ܷ�")]
    public float total_score;
    [Header("�����÷�ռ��")]
    public float basic_score_percent;
    [Header("��ʾ�ж����� [�༭������Ч]")]
    public bool note_show_touch_area;
}