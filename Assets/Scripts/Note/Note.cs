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
        Hold
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
                float y = rectTransform.position.y - Time.deltaTime * GameConst.drop_speed;
                rectTransform.position = new Vector2(x, y);
            }
        }

        public virtual void Init(Music.NoteCfg _cfg)
        {
            is_move = false;
            is_active = false;
            cfg = _cfg;

            Resize();
            ResetPosition();
        }

        public virtual void Activate()
        {
            is_active = true;
        }

        protected virtual void ResetPosition()
        {
            float x = (float)cfg.position_x * Screen.width;
            float y = Screen.height;
            rectTransform.position = new Vector2(x, y);
        }


        protected virtual void Resize()
        {
            Vector2 v = rectTransform.sizeDelta;
            v.y = GameConst.drop_speed * GameConst.active_interval * 2;
            rectTransform.sizeDelta = v;
            collider.size = rectTransform.sizeDelta;
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
        }

        /// <summary>
        /// ×¢Ïú·½·¨
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
                Miss();
            }
        }
    }
}

