using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JudgeInterval
{
    [Header("perfect�ж�����(s)")]
    public float perfect_interval;
    [Header("good�ж�����(s)")]
    public float good_interval;
    [Header("���ж�����(s)")]
    public float active_interval;
}

[CreateAssetMenu(fileName ="GameCFG", menuName = "ScriptableObject/GameCFG", order =0)]
public class GameCFG : ScriptableObject
{
    [Serializable]
    public class JudgeIntervalConfig
    {
        public Note.NoteType noteType;
        public JudgeInterval judgeInterval;
    }
    [Header("����༭���Ļ��������ٶ�")]
    public float editor_drop_speed;
    [Header("������Ϸ�Ļ��������ٶ�")]
    public float drop_speed;
    [Header("�ж��ߵ�λ��")]
    public float judge_line_y;
    [Header("�ж���������")]
    [SerializeField]
    private List<JudgeIntervalConfig> judgeIntervalConfigs;
    [Header("��Ϸ�ܷ�")]
    public float total_score;
    [Header("�����÷�ռ��")]
    public float basic_score_percent;
    [Header("Hold�����ж�����ӳ�")]
    public float hold_touch_area_width_extend;

    [Header("��ʾ�ж����� [�༭������Ч]")]
    public bool note_show_touch_area;
    [Header("hold_note���� [�༭������Ч]")]
    public bool hold_note_drop;
    [Header("����note���� [�༭������Ч]")]
    public bool note_drop;
    [Header("���ò���ģʽ")]
    public bool enable_test_mode;

    public Dictionary<Note.NoteType, JudgeInterval> JudgeIntervalConfigs { get; private set; }
    public void LoadJudgeIntervalConfigs()
    {
        JudgeIntervalConfigs = new Dictionary<Note.NoteType, JudgeInterval>();
        foreach (var cfg in judgeIntervalConfigs)
        {
            JudgeIntervalConfigs[cfg.noteType] = cfg.judgeInterval;
        }
    }
}
