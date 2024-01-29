using Note;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldNew : NoteBase, IPointerDownHandler
{
    private HoldPolygonImage touch_area;
    private HoldPolygonImage icon;
    Music.NoteCfg noteCfg;

    protected override void Awake()
    {
        base.Awake();
        touch_area = this.GetComponent<HoldPolygonImage>();
        icon = transform.Find("icon").GetComponent<HoldPolygonImage>();

        noteCfg = new Music.NoteCfg();
        /*        noteCfg.AddCheckPoint(1, 0.1);
                noteCfg.AddCheckPoint(2, 0.9);
                noteCfg.AddCheckPoint(3, 0.1);
                noteCfg.AddCheckPoint(4, 0.9);*/

        noteCfg.AddCheckPoint(1, 0.6, 0.7);
        noteCfg.AddCheckPoint(2, 0.5, 0.8);
        noteCfg.AddCheckPoint(3, 0.6, 0.9);
        is_move = false;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        Init(noteCfg, 0);

    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void Resize()
    {
        float drop_speed = 500;
        icon.SetCheckPoints(noteCfg.checkPoints, drop_speed);
        touch_area.SetCheckPoints(noteCfg.checkPoints, drop_speed, 40, 0.2f);

    }

    protected override void ResetPosition(float delta_time)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
}
