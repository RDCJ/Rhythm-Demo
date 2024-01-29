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
    [Header("perfect�ж�����")]
    public float perfect_interval;
    [Header("good�ж�����")]
    public float good_interval;
    [Header("bad�ж�����")]
    public float active_interval;
    [Header("��Ϸ�ܷ�")]
    public float total_score;
    [Header("�����÷�ռ��")]
    public float basic_score_percent;
}
