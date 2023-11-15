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

        [HideInInspector] public bool is_active;

        protected Music.NoteCfg cfg;
        protected RectTransform rectTransform;
        protected RectTransform judgeTigger_rect;

        protected bool is_move;

        protected virtual void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            judgeTigger_rect = transform.Find("JudgeTrigger").GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        public virtual void Init(Music.NoteCfg _cfg)
        {
            is_move = false;
            is_active = false;
            cfg = _cfg;
            ResetPosition();
        }

        public void Activate()
        {
            is_active = true;
            OnActive();
        }
        protected virtual void OnActive()
        {

        }

        protected void ResetPosition()
        {
            float x = (float)cfg.position_x * Screen.width;
            float y = Screen.height + rectTransform.sizeDelta.y;
            rectTransform.position = new Vector2(x, y);
        }

        public virtual void Drop()
        {
            is_move = true;
            GameMgr.Instance.pause_action += Pause;
            GameMgr.Instance.continue_action += Continue;
            StartCoroutine(_Drop());
        }

        private IEnumerator _Drop()
        {
            float x, y;
            while (true)
            {
                if (!is_move)
                    yield return new WaitWhile(() => !is_move);
                x = rectTransform.position.x;
                y = rectTransform.position.y - Time.deltaTime * GameConst.drop_speed;
                rectTransform.position = new Vector2(x, y);
                yield return null;
            }
        }

        public virtual void Pause()
        {
            is_move = false;
        }
        public virtual void Continue()
        {
            is_move = true;
        }

        public virtual void Miss() { }

        /// <summary>
        /// ×¢Ïú·½·¨
        /// </summary>
        public void OnReturnPool()
        {
            GameMgr.Instance.pause_action -= Pause;
            GameMgr.Instance.continue_action -= Continue;
        }
    }
}

