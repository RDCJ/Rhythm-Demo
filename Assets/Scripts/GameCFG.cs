using UnityEngine;

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
    

}
