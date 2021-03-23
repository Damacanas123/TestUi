using System;
using Helper.InputHelper;
using Slime.Helper.DI;
using Slime.Helper.Extensions;
using Slime.Helper.Geometry;
using UnityEngine;

namespace Slime.Helper.InputHelper
{
    public class Touchable : MonoBehaviour
    {
        private bool TouchDownSubscribed;
        private bool TouchUpSubscribed;
        private bool SwipeSubscribed;

        private BoxCollider2D _boxCollider;
        //backwards compatibility in order to set boundaing box both from code and unity editor
        private bool BoundingBoxHasSet;
        private SpriteRenderer maxSortingPriorityRenderer;
        private bool _pressed;
        private SpriteRenderer _maxSortingPriorityRenderer
        {
            get
            {
                if (maxSortingPriorityRenderer == null)
                {
                    var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
                    maxSortingPriorityRenderer = GetMax(spriteRenderers);
                }

                return maxSortingPriorityRenderer;
            }
        }
        private TouchHelper touchHelper;

        private TouchHelper _touchHelper
        {
            get
            {
                if (touchHelper == null)
                {
                    touchHelper = DIContainer.GetSingle<TouchHelper>();
                }

                return touchHelper;
            }
        }
        private int _currentTouchFingerID;
        void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        SpriteRenderer GetMax(SpriteRenderer[] spriteRenderers)
        {
            if (spriteRenderers.Length < 1)
            {
                return null;
            }

            var max = spriteRenderers[0];
            for(int i = 1;i < spriteRenderers.Length; i++)
            {
                var spriteRenderer = spriteRenderers[i];
                if (max.CompareTo(spriteRenderer) < 0)
                {
                    max = spriteRenderer;
                }
            }

            return max;
        }

        public void SetOnTouchDown(Func<TouchDownEventArgs, bool> callback)
        {
            var wrapperCallback = new Func<TouchDownEventArgs, bool>(args =>
            {
                if (_boxCollider.bounds.Contains(args.position) && !_pressed)
                {
                    _currentTouchFingerID = args.touchFingerID;
                    _pressed = true;
                    return callback(args);    
                }
                return true;
            });
            _touchHelper.SubscribeOnTouchDown(wrapperCallback,_maxSortingPriorityRenderer,GetInstanceID());
            TouchDownSubscribed = true;
        }

        public void SetOnTouchUp(Action<TouchUpEventArgs> callback)
        {
            var wrapperCallback = new Action<TouchUpEventArgs>(args =>
            {
                if (args.touchFingerID == _currentTouchFingerID && _pressed)
                {
                    _pressed = false;
                    callback(args);
                }
            });
            _touchHelper.SubscribeOnTouchUp(wrapperCallback,GetInstanceID());
            TouchUpSubscribed = true;
        }

        public bool IsInside(Vector2 point)
        {
            return _boxCollider.bounds.Contains(point);
        }

        
        public void SetOnSwipe(Action<SwipeEventArgs> callback)
        {
            var wrapperCallback = new Action<SwipeEventArgs>(args =>
            {
                if (_boxCollider.bounds.Contains(args.origPosition))
                {
                    callback(args);
                }
            });
            SwipeSubscribed = true;
            _touchHelper.SubscribeSwipe(wrapperCallback,GetInstanceID());
        }

        private void OnDestroy()
        {
            if (TouchDownSubscribed)
            {
                _touchHelper.UnsubscribeTouchDown(GetInstanceID());    
            }

            if (TouchUpSubscribed)
            {
                _touchHelper.UnsubscribeTouchUp(GetInstanceID());    
            }

            if (SwipeSubscribed)
            {
                _touchHelper.UnsubscribeSwipe(GetInstanceID());
            }
            
            
        }
    }
}