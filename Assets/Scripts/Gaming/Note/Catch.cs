using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Note;

/// <summary>
/// �������ж���С������perfect
/// </summary>
public class Catch : NoteBase, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler
{
    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Catch;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.is_active && !this.is_judged)
        {
            Judge();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (this.is_active && !this.is_judged)
        {
            Judge();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active && !this.is_judged)
        {
            Judge();
        }
    }

    protected override void Resize()
    {
        float touch_area_length = DropSpeedFix.GetScaledDropSpeed * GameConst.good_interval * 2;
        rectTransform.sizeDelta = Util.ChangeV2(rectTransform.sizeDelta, 1, touch_area_length);
        collider.size = rectTransform.sizeDelta;
    }

    private void Judge()
    {
        is_judged = true;

        ScoreMgr.ScoreLevel level = ScoreMgr.ScoreLevel.perfect;
        // �Ʒ�
        Debug.Log("[�ж�] ʱ��: " + GameMgr.Instance.current_time.ToString("N4") + "����: Catch, ���: " + level);
        ScoreMgr.Instance.AddScore(level);
        // ���Ч��
        PlayEffect(level);
        //
        NotePoolManager.Instance.ReturnObject(this);
    }
}
