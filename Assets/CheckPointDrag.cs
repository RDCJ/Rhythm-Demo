using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckPointDrag : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    EditorHoldPainter hold;

    public RectTransform rectTransform;

    public int index;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("CheckPointDrag OnDrag");
        hold.OnCheckPointDrag(eventData, index, rectTransform);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[CheckPointDrag] OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("[CheckPointDrag] OnPointerExit");
    }
}
