using System;
using Unity.VisualScripting;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    #region Singleton
    private LoadingScreenManager() { }
    private static LoadingScreenManager instance;
    public static LoadingScreenManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    private Animator _animatorComponent;
    private bool loading_end;
    private Action onFinishShow = null;
    private Action onFinishHide = null;
    private Action onFinishIdle = null;

    private void Awake()
    {
        instance = this;
        _animatorComponent = transform.GetComponent<Animator>();
    }

    private void Start()
    {
        loading_end = true;
    }

    public void StartLoading(Action _onFinishShow=null)
    {
        _animatorComponent.SetTrigger("Reveal");
        onFinishShow = _onFinishShow;
        loading_end = false;
    }

    public void EndLoading(Action _onFinishIdle = null, Action _onFinishHide = null)
    {
        loading_end = true;
        onFinishIdle = _onFinishIdle;
        onFinishHide = _onFinishHide;
    }

    public void OnFinishShow()
    {
        onFinishShow?.Invoke();
        onFinishShow = null;
    }

    public void OnFinishIdle()
    {
        if (loading_end)
        {
            _animatorComponent.SetTrigger("Hide");
            onFinishIdle?.Invoke();
            onFinishIdle = null;
        }
    }

    public void OnFinishHide()
    {
        onFinishHide?.Invoke();
        onFinishHide = null;
    }
}
