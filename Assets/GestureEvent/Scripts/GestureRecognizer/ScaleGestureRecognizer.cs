using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class ScaleMessage : IGestureMessage
    {
        public int fingerId_1;
        public int fingerId_2;
        public float previousDistance;
        public float currentDistance;

        public ScaleMessage(int fingerId_1, int fingerId_2, float previousDistance, float currentDistance)
        {
            this.fingerId_1 = fingerId_1;
            this.fingerId_2 = fingerId_2;
            this.previousDistance = previousDistance;
            this.currentDistance = currentDistance;
        }
    }

    /// <summary>
    /// Detect two fingers moving toward or away
    /// </summary>
    [CreateAssetMenu(fileName = "ScaleGestureRecognizer", menuName = "GestureRecognizer/ScaleGestureRecognizer")]
    public class ScaleGestureRecognizer : GestureRecognizer
    {
        private List<Touch> previousTwoFingers = null;

        public override void Recognize(TouchesInfo touchesInfo)
        {
            if (touchesInfo.touchCount != 2)
            {
                previousTwoFingers = null;
                return;
            }
            var phaseDic = touchesInfo.phaseDic;
            List<Touch> currentTwoFingers = new List<Touch>(2);
            foreach (var phase in new List<TouchPhase>() { TouchPhase.Stationary, TouchPhase.Moved })
            {
                if (!phaseDic.ContainsKey(phase)) continue;
                foreach (var touch in phaseDic[phase])
                {
                    currentTwoFingers.Add(touch);
                }
            }
            if (currentTwoFingers.Count != 2)
            {
                previousTwoFingers = null;
                return;
            }
            currentTwoFingers.Sort((touch1, touch2) => touch1.fingerId < touch2.fingerId ? 1 : -1);

            if (previousTwoFingers != null)
            {
                if (previousTwoFingers[0].fingerId == currentTwoFingers[0].fingerId && previousTwoFingers[1].fingerId == currentTwoFingers[1].fingerId)
                {
                    float previousDistance = (previousTwoFingers[0].position - previousTwoFingers[1].position).magnitude;
                    float currentDistance = (currentTwoFingers[0].position - currentTwoFingers[1].position).magnitude;
                    DispatchEvent(new ScaleMessage(currentTwoFingers[0].fingerId, currentTwoFingers[1].fingerId, previousDistance, currentDistance));
                }
            }
            previousTwoFingers = currentTwoFingers;

        }
    }
}

