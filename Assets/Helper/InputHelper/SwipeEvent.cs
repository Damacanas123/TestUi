using System;
using UnityEngine;

namespace Slime.Helper.InputHelper
{
    public class SwipeEventArgs : EventArgs
    {
        public Vector2 origPosition { get; set; }
        public Vector2 lastPosition { get; set; }
        public Vector2 lastNormalizedPosition { get; set; }
        public Vector2 delta { get; set; }
        public SwipeStatus swipeStatus { get; set; }
        public int fingerId { get; set; }

        public SwipeEventArgs(Vector2 origPosition,Vector2 lastPosition,Vector2 lastNormalizedPosition, Vector2 delta,SwipeStatus swipeStatus,int fingerId)
        {
            this.origPosition = origPosition;
            this.lastPosition = lastPosition;
            this.lastNormalizedPosition = lastNormalizedPosition;
            this.delta = delta;
            this.swipeStatus = swipeStatus;
            this.fingerId = fingerId;
        }
    }
    public delegate void SwipeEvent(SwipeEventArgs args);
}