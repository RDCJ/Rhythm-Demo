using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public GameObject prefect_prefab;
    public GameObject good_prefab;
    public GameObject bad_prefab;
    public float effect_time;
    public float scale;

    private void Awake()
    {
        instance = this;
    }

    public void PlayEffect(ScoreMgr.ScoreLevel level, Vector3 position)
    {
        GameObject new_effect = null;
        switch (level)
        {
            case ScoreMgr.ScoreLevel.perfect:
                new_effect = Instantiate(prefect_prefab, transform);
                break;
            case ScoreMgr.ScoreLevel.good:
                new_effect = Instantiate(good_prefab, transform);
                break;
            case ScoreMgr.ScoreLevel.bad:
                new_effect = Instantiate(bad_prefab, transform);
                break;
        }
        new_effect.transform.position = position;
        new_effect.transform.DOScale(new Vector3(scale, scale, 0), effect_time);
        new_effect.GetComponent<CanvasGroup>().DOFade(0, effect_time);
        Destroy(new_effect, effect_time);
    }
}
