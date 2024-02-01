using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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
        protected BoxCollider2D collider;

        protected bool is_move;
        protected int finger_count;
        [HideInInspector] protected NoteState state;
        [HideInInspector] protected bool is_judged;

        public bool IsActive
        {
            get => state != NoteState.Inactive;
        }
        public bool IsJudged
        {
            get => state == NoteState.Judged;
        }

        public bool IsHolding
        {
            get => finger_count > 0;
        }

        protected virtual void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            collider = this.GetComponent<BoxCollider2D>();
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
                float x = rectTransform.position.x;
                float y = rectTransform.position.y - Time.deltaTime * DropSpeedFix.GetScaledDropSpeed;
                rectTransform.position = new Vector2(x, y);
            }
        }

        protected virtual void FixedUpdate()
        {
            float distance_to_judge_line = rectTransform.position.y - Screen.width * GameConst.judge_line_y;
            if (!IsActive)
            {
                if (distance_to_judge_line > 0 && distance_to_judge_line < DropSpeedFix.GetScaledDropSpeed * GameConst.active_interval)
                {
                    state = NoteState.active;
                    Debug.Log("note active");
                }
            }
            else if (distance_to_judge_line < -DropSpeedFix.GetScaledDropSpeed * GameConst.active_interval)
            {
                if (!IsJudged)
                {
                    Miss();
                    Debug.Log("note miss");
                }
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
            finger_count = 0;
            state = NoteState.Inactive;
            cfg = _cfg;

            Resize();
            ResetPosition(delta_time);
        }

        public virtual void Activate()
        {
            state = NoteState.active;
        }

        /// <summary>
        /// 调整点击区域和collider的大小
        /// </summary>
        protected virtual void Resize()
        {
            float touch_area_length = DropSpeedFix.GetScaledDropSpeed * GameConst.active_interval * 2 / MainCanvas.Instance.GetScaleFactor;
            rectTransform.sizeDelta = Util.ChangeV2(rectTransform.sizeDelta, 1, touch_area_length);
            collider.size = rectTransform.sizeDelta;
        }

        /// <summary>
        /// 重置位置
        /// </summary>
        /// <param name="delta_time">用于修正生成时刻的偏差</param>
        protected virtual void ResetPosition(float delta_time)
        {
            float x = (float)cfg.FirstCheckPoint().Center() * Screen.width;
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

/*        private void OnTriggerEnter2D(Collider2D collision)
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
                if (!IsJudged)
                    Miss();
            }
        }*/

        public void PlayEffect(ScoreMgr.ScoreLevel scoreLevel)
        {
            float x = this.transform.position.x;
            float y = JudgeLine.Instance.transform.position.y;
            EffectPlayer.Instance.PlayEffect(scoreLevel, new Vector3(x, y, 0));
        }
    }
}

