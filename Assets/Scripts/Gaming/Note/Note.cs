using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public abstract class NoteBase : MonoBehaviour
    {
        protected NoteType type;
        public NoteType Type
        { get => type; private set => type = value; }

        protected Music.NoteCfg cfg;
        protected RectTransform rectTransform;
        protected BoxCollider2D collider;

        protected bool is_move;
        [HideInInspector] protected bool is_active;
        [HideInInspector] protected bool is_judged;

        protected virtual void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            collider = this.GetComponent<BoxCollider2D>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (is_move)
            {
                float x = rectTransform.position.x;
                float y = rectTransform.position.y - Time.deltaTime * DropSpeedFix.GetScaledDropSpeed;
                rectTransform.position = new Vector2(x, y);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_cfg"></param>
        /// <param name="delta_time">初始化的时刻与理论时刻的偏差，用于修正位置</param>
        public virtual void Init(Music.NoteCfg _cfg, float delta_time)
        {
            is_move = false;
            is_active = false;
            is_judged = false;
            cfg = _cfg;

            Resize();
            ResetPosition(delta_time);
        }

        public virtual void Activate()
        {
            Debug.Log("Activate");
            is_active = true;
        }

        /// <summary>
        /// 调整点击区域和collider的大小
        /// </summary>
        protected virtual void Resize()
        {
            float touch_area_length = DropSpeedFix.GetScaledDropSpeed * GameConst.active_interval * 2;
            rectTransform.sizeDelta = Util.ChangeV2(rectTransform.sizeDelta, 1, touch_area_length);
            collider.size = rectTransform.sizeDelta;
        }

        /// <summary>
        /// 重置位置
        /// </summary>
        /// <param name="delta_time">用于修正生成时刻的偏差</param>
        protected virtual void ResetPosition(float delta_time)
        {
            float x = (float)cfg.position_x * Screen.width;
            float y = Screen.height + delta_time * DropSpeedFix.GetScaledDropSpeed;
            rectTransform.position = new Vector2(x, y);
        }

        public virtual void Drop()
        {
            is_move = true;
            GameMgr.Instance.pause_action += Pause;
            GameMgr.Instance.continue_action += Continue;
        }


        public virtual void Pause()
        {
            is_move = false;
        }
        public virtual void Continue()
        {
            is_move = true;
        }


        public virtual void Miss() {
            ScoreMgr.Instance.AddScore(ScoreMgr.ScoreLevel.bad);
            NotePoolManager.Instance.ReturnObject(this);
        }

        /// <summary>
        /// 注销方法
        /// </summary>
        public void OnReturnPool()
        {
            GameMgr.Instance.pause_action -= Pause;
            GameMgr.Instance.continue_action -= Continue;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("JudgeLine"))
            {
                Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("JudgeLine"))
            {
                if (!is_judged)
                    Miss();
            }
        }

        public void PlayEffect(ScoreMgr.ScoreLevel scoreLevel)
        {
            float x = this.transform.position.x;
            float y = JudgeLine.Instance.transform.position.y;
            EffectPlayer.Instance.PlayEffect(scoreLevel, new Vector3(x, y, 0));
        }
    }
}

