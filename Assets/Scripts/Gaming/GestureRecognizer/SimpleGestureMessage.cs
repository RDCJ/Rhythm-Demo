using GestureEvent;
using UnityEngine;

namespace NoteGesture
{
    public class SimpleGestureMessage : IGestureMessage
    {
        public int fingerId;
        public Vector2 position;
        public RaycastHit2D hit = default;

        public SimpleGestureMessage(int fingerId, Vector2 position)
        {
            this.fingerId = fingerId;
            this.position = position;
        }

        public SimpleGestureMessage(int fingerId, Vector2 position, RaycastHit2D hit)
        {
            this.fingerId = fingerId;
            this.position = position;
            this.hit = hit;
        }
    }
}