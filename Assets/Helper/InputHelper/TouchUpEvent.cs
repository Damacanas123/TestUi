using System;
using UnityEngine;

namespace Slime.Helper.InputHelper
{
    public class TouchUpEventArgs : EventArgs
    {
        //normalized to camera, useful for scenes that is bigger than camera view
        public Vector2 normalizedPosition { get; set; }
        //world position
        public Vector2 position { get; set; }
        public int touchFingerID { get; set; }

        public TouchUpEventArgs(Vector2 normalizedPosition, Vector2 position,int touchFingerID)
        {
            this.normalizedPosition = normalizedPosition;
            this.position = position;
            this.touchFingerID = touchFingerID;
        }
    }
    public delegate void TouchUpEvent(TouchUpEventArgs args);
}