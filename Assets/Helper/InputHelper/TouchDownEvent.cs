using System;
using UnityEngine;

namespace Slime.Helper.InputHelper
{
    public class TouchDownEventArgs : EventArgs
    {
        public Vector2 normalizedPosition { get; set; }
        public Vector2 position { get; set; }
        public int touchFingerID { get; set; }

        public TouchDownEventArgs(Vector2 normalizedPosition, Vector2 position, int touchFingerID)
        {
            this.normalizedPosition = normalizedPosition;
            this.position = position;
            this.touchFingerID = touchFingerID;
        }
    }
    public delegate void TouchDownEvent(TouchDownEventArgs args);
}