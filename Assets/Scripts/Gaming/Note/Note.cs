using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEditor;
using Music;

namespace Note
{
    public enum NoteType
    {
        Tap,
        LeftSlide,
        RightSlide,
        Hold,
        Catch
    }

    public enum NoteState
    {
        Inactive,
        active,
        Judged
    }

    public abstract class NoteBase : MonoBehaviour
    {
        protected NoteType type;
        public NoteType Type
        { get => type; private set => type = value; }

        protected Music.NoteCfg cfg;
        protected RectTransform rectTransform;
        protected RectTransform icon_rtf;
        protected CanvasGroup icon_canvasGroup;

        protected bool is_move;
        protected bool icon_is_fade;
        [HideInInspector] protected NoteState state;
        [HideInInspector] protected bool is_judged;
        protected GestureEvent.GestureMgr gestureMgr;

        public bool IsActive
        {
            get => state != NoteState.Inactive;
        }
        public bool IsJudged
        {
            get => state == NoteState.Judged;
        }

        protected virtual void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            icon_rtf = transform.Find("icon").GetComponent<RectTransform>();
            icon_canvasGroup = icon_rtf.GetComponent<CanvasGroup>();
#if UNITY_EDITOR
            Image touch_img = this.GetComponent<Image>();
            if (GameConst.note_show_touch_area)
            {
                var color = touch_img.color;
                color.a = 0.5f;
                touch_img.color = color;
            }
#endif
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (is_move)
            {
                float x = rectTransform.localPosition.x;
                float y = rectTransform.localPosition.y - Time.deltaTime * PlayerPersonalSetting.ScaledDropSpeed;
                rectTransform.localPosition = new Vector3(x, y, 0);

                float distance_to_judge_line = rectTransform.localPosition.y + Screen.height * 0.5f - JudgeLine.localPositionY;
                // note进入判定区
                if (!IsActive)
                {
                    if (distance_to_judge_line > 0 && distance_to_judge_line < TouchAreaLength * 0.5f)
                    {
                        Activate();
                    }
                }
                // note离开判定区
                else if (distance_to_judge_line < - TouchAreaLength * 0.5f)
                {
                    EndJudge();
                    if (!IsJudged)
                    {
                        Miss();
                    }
                }

                if (!icon_is_fade && distance_to_judge_line < 0)
                {
                    icon_is_fade = true;
                    IconFade();
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_cfg"></param>
        /// <param name="delta_time">初始化的时刻与理论时刻的偏差，用于修正位置</param>
        public virtual void Init(Music.NoteCfg _cfg, float delta_time, GestureEvent.GestureMgr gestureMgr)
        {
            is_move = false;
            state = NoteState.Inactive;
            cfg = _cfg;
            this.gestureMgr = gestureMgr;

            icon_is_fade = false;
            icon_canvasGroup.DOKill();
            icon_canvasGroup.alpha = 1.0f;

            Resize();
            ResetPosition(delta_time);
        }

        public virtual void Activate()
        {
            state = NoteState.active;
            if (gestureMgr != null)
                RegisterGestureHandler();
        }

        /// <summary>
        /// 调整点击区域的大小
        /// </summary>
        protected virtual void Resize()
        {
            rectTransform.sizeDelta = Util.ChangeV2(rectTransform.sizeDelta, 1, TouchAreaLength);
        }

        /// <summary>
        /// 重置位置
        /// </summary>
        /// <param name="delta_time">用于修正生成时刻的偏差</param>
        protected virtual void ResetPosition(float delta_time)
        {
            float x = (float)(cfg.FirstCheckPoint().Center() - 0.5f) * Screen.width;
            float y = delta_time * PlayerPersonalSetting.ScaledDropSpeed;
            rectTransform.localPosition = new Vector3(x, y, 0);
        }

        public virtual void Drop()
        {
            is_move = true;
            GameMgr.Instance.OnGamePause += Pause;
            GameMgr.Instance.OnGameContinue += Continue;
        }

        public virtual void Pause()
        {
            is_move = false;
        }
        public virtual void Continue()
        {
            is_move = true;
        }

        protected virtual void IconFade()
        {
            icon_canvasGroup.DOFade(0.3f, 0.1f);
        }

        protected virtual void EndJudge() { }

        public virtual void Miss() {
            Debug.Log(this.GetType().Name + " Miss");
            PlayEffect(ScoreMgr.ScoreLevel.bad);
            GameMgr.Instance.AddScore(ScoreMgr.ScoreLevel.bad);
            NotePoolManager.Instance.ReturnObject(this);
        }

        /// <summary>
        /// 注销方法
        /// </summary>
        public void OnReturnPool()
        {
            GameMgr.Instance.OnGamePause -= Pause;
            GameMgr.Instance.OnGameContinue -= Continue;
            if (gestureMgr != null)
                UnregisterGestureHandler();
        }


        public void PlayEffect(ScoreMgr.ScoreLevel scoreLevel)
        {
            float x = GetCenterXOnJudgeLine;
            if (Mathf.Abs(x) < Screen.width * 0.5f)
            {
                float y = JudgeLine.localPositionY;
                EffectPlayer.Instance.PlayEffect(scoreLevel, new Vector3(x, y, 0));
            }
            else
            {
                Debug.Log("[Note.PlayEffect] position_x is out of screen: " + x);
            }
            
        }

        /// <summary>
        /// 判定区域的长度
        /// </summary>
        protected virtual float TouchAreaLength
        {
            get 
            { 
                return PlayerPersonalSetting.ScaledDropSpeed * GameConst.active_interval * 2;
            }
        }

        protected virtual CheckPoint GetCheckPointOnJudgeLine
        {
            get
            {
                double time = GameMgr.Instance.CurrentTime;
                int n = cfg.checkPoints.Count;
                if (time < cfg.FirstCheckPoint().time)
                {
                    return new CheckPoint(cfg.FirstCheckPoint());
                }
                for (int i = 0; i < n - 1; i++)
                {
                    var ckp1 = cfg.checkPoints[i];
                    var ckp2 = cfg.checkPoints[i + 1];
                    if (time >= ckp1.time && time <= ckp2.time)
                    {
                        double k = (time - ckp1.time) / (ckp2.time - ckp1.time);
                        double l = ckp1.position_l + k * (ckp2.position_l - ckp1.position_l);
                        double r = ckp1.position_r + k * (ckp2.position_r - ckp1.position_r);
                        return new CheckPoint(time, l, r);
                    }
                }
                return new CheckPoint(cfg.LastCheckPoint());
            }
        }

        protected virtual float GetCenterXOnJudgeLine
        {
            get => this.transform.localPosition.x;
        }

        protected virtual void RegisterGestureHandler() { }
        protected virtual void UnregisterGestureHandler() { }
    }
}

