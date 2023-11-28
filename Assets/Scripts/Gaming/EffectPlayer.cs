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
        GameObject new_effect = pools[level].GetObject();
        CanvasGroup canvasGroup = new_effect.GetComponent<CanvasGroup>();
        new_effect.transform.position = position;
        new_effect.transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
        new_effect.transform.DOScale(new Vector3(scale, scale, 1), effect_time);
        canvasGroup.DOFade(0, effect_time).OnComplete(() =>
        {
            pools[level].ReturnObject(new_effect);
        });
    }
}
