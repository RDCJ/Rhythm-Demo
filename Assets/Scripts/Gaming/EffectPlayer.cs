using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectPlayer : MonoBehaviour
{
    #region Singleton
    private EffectPlayer() { }
    private static EffectPlayer instance;
    public static EffectPlayer Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
    Dictionary<ScoreMgr.ScoreLevel, ObjectPool> pools;
    public float effect_time;
    public float scale;

    private void Awake()
    {
        instance = this;
        pools = new Dictionary<ScoreMgr.ScoreLevel, ObjectPool>
        {
            {
                ScoreMgr.ScoreLevel.perfect,
                new ObjectPool(5, FileConst.perfect_effect_prefab_path, transform)
            },
            {
                ScoreMgr.ScoreLevel.good,
                new ObjectPool(5, FileConst.good_effect_prefab_path, transform)
            },
            {
                ScoreMgr.ScoreLevel.bad,
                new ObjectPool(5, FileConst.bad_effect_prefab_path, transform)
            },
        };
    }

    public void PlayEffect(ScoreMgr.ScoreLevel level, Vector3 position)
    {
        //Debug.Log("[EffectPlayer.PlayEffect]: " + position);
        GameObject new_effect = pools[level].GetObject();
        new_effect.GetComponent<RectTransform>().anchoredPosition = position;
        new_effect.transform.Find("Light").localScale = Vector3.one * scale;
        new_effect.transform.Find("Star").localScale = Vector3.one * scale;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(effect_time).AppendCallback(() => { pools[level].ReturnObject(new_effect); });
    }
}
