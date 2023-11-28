using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldIcon : MonoBehaviour
{
    RectTransform trans;
    BoxCollider2D collider2D;
    bool f;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
        collider2D = GetComponent<BoxCollider2D>();
    }

/*    public void IconUpdate()
    {
        if (!f && trans.sizeDelta.y > 0)
        {
            float y = trans.sizeDelta.y - Time.deltaTime * DropSpeedFix.GetScaledDropSpeed;
            trans.sizeDelta = Util.ChangeV2(trans.sizeDelta, 1, y);
        }
    }*/


    public void Resize(float icon_length)
    {
        trans.sizeDelta = Util.ChangeV2(trans.sizeDelta, 1, icon_length);

        trans.localPosition = Util.ChangeV3(trans.localPosition, 1, icon_length / 2);
        collider2D.size = trans.sizeDelta;
        collider2D.offset = new Vector2(0, -icon_length / 2);
    }

/*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("JudgeLine"))
        {
            f = false;
            Debug.Log("OnTriggerEnter2D: JudgeLine");
        }
    }*/

    public Vector2 sizeDelta
    {
        get { return trans.sizeDelta; }
    }
}
