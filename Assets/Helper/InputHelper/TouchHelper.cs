using System;
using System.Collections.Generic;
using Slime.Helper.Context;
using Slime.Helper.GameHelper;
using Slime.Helper.InputHelper;
using UnityEngine;

namespace Helper.InputHelper
{
    
    public class TouchHelper :IManualUpdate
    {
        private class TouchData
        {
            public Touch touch;
            public int touchFingerID;
            public int touchIndex;
        }

        private struct TouchDownSubscriber
        {
            public int instanceID;
            public Func<TouchDownEventArgs, bool> callback;
            public int sortingLayerValue;
            public int sortingOrder;
        }

        private struct TouchUpSubscriber
        {
            public int instanceID;
            public Action<TouchUpEventArgs> callback;
        }
        
        private Dictionary<int,TouchData> _activeTouches = new Dictionary<int, TouchData>();

        public Vector2 GetCurrentTouchPosition(int touchFingerIndex)
        {
            Vector2 position;
#if UNITY_EDITOR
            position = Input.mousePosition;
#else
            var touchIndex = _activeTouches[touchFingerIndex].touchIndex;
            position = Input.GetTouch(touchIndex).position;    
#endif
            
                position = _coordinateHelper.ScreenToWorldPointNormalizedToCamera(position);
                return position;
        }
        

        bool CompareOrder(SpriteRenderer renderer, TouchDownSubscriber subscriber)
        {
            int rendererLayerValue = SortingLayer.GetLayerValueFromID(renderer.sortingLayerID);
            int subscriberLayerValue = subscriber.sortingLayerValue;
            int layerCompare = rendererLayerValue.CompareTo(subscriberLayerValue);
            if (layerCompare == 0)
            {
                return renderer.sortingOrder.CompareTo(subscriber.sortingOrder) > 0;
            }

            return layerCompare > 0;
        }

        private List<TouchDownSubscriber> OnTouchDownSubscribers = new List<TouchDownSubscriber>();

        public void SubscribeOnTouchDown(Func<TouchDownEventArgs, bool> callback,SpriteRenderer renderer,int instanceID)
        {
            //insert into subscribers that is reverse sorted
            //find correct index
            int i = 0;
            for (; i < OnTouchDownSubscribers.Count; i++)
            {
                var subscriber = OnTouchDownSubscribers[i];
                if (CompareOrder(renderer, subscriber))
                {
                    break;
                }
            }
            OnTouchDownSubscribers.Insert(i,new TouchDownSubscriber()
            {
                callback = callback,
                sortingLayerValue = SortingLayer.GetLayerValueFromID(renderer.sortingLayerID),
                sortingOrder = renderer.sortingOrder,
                instanceID = instanceID
            });
        }
        
        private List<TouchUpSubscriber> OnTouchUpSubscribers = new List<TouchUpSubscriber>();

        public void SubscribeOnTouchUp(Action<TouchUpEventArgs> callback,int instanceID)
        {
            OnTouchUpSubscribers.Add(new TouchUpSubscriber()
            {
                callback = callback,
                instanceID = instanceID
            });
        }

        public void UnsubscribeTouchDown(int instanceID)
        {
            OnTouchDownSubscribers.RemoveAll(o => o.instanceID == instanceID);
        }
        
        public void UnsubscribeTouchUp(int instanceID)
        {
            OnTouchUpSubscribers.RemoveAll(o => o.instanceID == instanceID);
        }
        
        
        private CoordinateHelper _coordinateHelper;
        
        

        public TouchHelper(CoordinateHelper coordinateHelper)
        {
            _coordinateHelper = coordinateHelper;
        }

        

        public (TouchPhase, Vector2) GetMouseTouch(out bool isClicked)
        {
            isClicked = true;
            TouchPhase touchPhase = TouchPhase.Began;
            Vector2 position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                touchPhase = TouchPhase.Began;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                touchPhase = TouchPhase.Ended;
            }
            else if (Input.GetMouseButton(0))
            {
                touchPhase = TouchPhase.Moved;
            }
            else
            {
                isClicked = false;
            }
            

            return (touchPhase, position);
        }

        
        public void ManualUpdate()
        {
            List<TouchData> touches = new List<TouchData>();
            TouchPhase touchPhase;
            Vector2 position;
            Touch touch;
            
#if UNITY_EDITOR
            (touchPhase, position) = GetMouseTouch(out bool isClicked);
            
            if (!isClicked)
            {
                return;
            }
            touch = new Touch()
            {
                phase = touchPhase,
                position = position
            };
            
            touches.Add(new TouchData()
            {
                touch = touch,
                touchFingerID = 0,
                touchIndex = 0
            });
#else
            if (Input.touchCount < 1)
            {
                return;
            }
            for (int touchNumber = 0; touchNumber < Input.touchCount; touchNumber++)
            {
                touch = Input.GetTouch(touchNumber);
                touches.Add((new TouchData()
                {
                    touch = touch,
                    touchFingerID = touch.fingerId,
                    touchIndex = touchNumber
                }));
            }
#endif
            
            _activeTouches.Clear();
            foreach (var touchData in touches)
            {
                _activeTouches[touchData.touchFingerID] = touchData;
                HandleTouch(touchData.touch);
            }
        }
        
        private float swipeThreshold = 0.2f;

        private class SwipeData
        {
            public bool isSwiping;
            public Vector3 origPosition;
            public Vector3 lastPosition;
        }
        /// <summary>
        /// keys are fingerIDs
        /// </summary>
        private Dictionary<int, SwipeData> _swipes = new Dictionary<int, SwipeData>(); 

        void HandleTouch(Touch touch)
        {
            var touchPhase = touch.phase;
            switch (touchPhase)
            {
                case TouchPhase.Began:
                    var result = OnTouchDown(touch);
                    if (result)
                    {
                        OnSwipeDown(touch);    
                    }
                    
                    break;
                case TouchPhase.Canceled:
                    OnTouchUp(touch);
                    break;
                case TouchPhase.Ended:
                    OnTouchUp(touch);
                    OnSwipeEnd(touch);
                    break;
                case TouchPhase.Moved:
                    OnSwipeMove(touch);
                    break;
                    
            }
        }

        bool OnTouchDown(Touch touch)
        {
            var touchFingerId = touch.fingerId;
            var normalizedPosition = _coordinateHelper.ScreenToWorldPointNormalizedToCamera(touch.position);
            var position = _coordinateHelper.ScreenToWorldPoint(touch.position);
            bool canContinue = true;
            for(int i = OnTouchDownSubscribers.Count - 1; i > -1; i--)
            {
                var subscriber = OnTouchDownSubscribers[i];
                var callback = subscriber.callback;
                if (canContinue)
                {
                    canContinue = callback(new TouchDownEventArgs(normalizedPosition, position, touchFingerId));
                    if (!canContinue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        void OnTouchUp(Touch touch)
        {
            var touchFingerId = touch.fingerId;
            var normalizedPosition = _coordinateHelper.ScreenToWorldPointNormalizedToCamera(touch.position);
            var position = _coordinateHelper.ScreenToWorldPoint(touch.position);
            for(int i = OnTouchUpSubscribers.Count - 1; i > -1; i--)
            {
                var subscriber = OnTouchUpSubscribers[i];
                subscriber.callback(new TouchUpEventArgs(normalizedPosition, position, touchFingerId));
            }
        }

        /// <summary>
        /// note that this method does not call subscriber events. Swipe is started after a certain distance threshold is reached
        /// </summary>
        /// <param name="touch"></param>
        void OnSwipeDown(Touch touch)
        {
            var position = _coordinateHelper.ScreenToWorldPoint(touch.position);
            _swipes[touch.fingerId] = new SwipeData()
            {
                origPosition = position,
                lastPosition = position
            };
        }

        void OnSwipeMove(Touch touch)
        {
            if (!_swipes.ContainsKey(touch.fingerId))
            {
                return;
            }
            var position = _coordinateHelper.ScreenToWorldPoint(touch.position);
            var normalizedPosition = _coordinateHelper.ScreenToWorldPointNormalizedToCamera(touch.position);
            var swipeData = _swipes[touch.fingerId];
            var delta = position - swipeData.lastPosition;
            if (swipeData.isSwiping)
            {
                foreach (var subscriber in _swipeSubscribers)
                {
                    subscriber.callback(new SwipeEventArgs(swipeData.origPosition,position, normalizedPosition,
                        delta, SwipeStatus.OnSwipe,touch.fingerId));
                }
            }
            else
            {
                if ((position - swipeData.origPosition).magnitude > swipeThreshold)
                {
                    swipeData.isSwiping = true;
                    foreach (var subscriber in _swipeSubscribers)
                    {
                        //call swipe start. its delta is current pos - swipe orig position
                        subscriber.callback(new SwipeEventArgs(swipeData.origPosition,position, normalizedPosition, 
                            position - swipeData.origPosition, SwipeStatus.Start,touch.fingerId));
                    }
                }
            }
            
            swipeData.lastPosition = position;
        }

        void OnSwipeEnd(Touch touch)
        {
            if (!_swipes.ContainsKey(touch.fingerId))
            {
                return;
            }
            var position = _coordinateHelper.ScreenToWorldPoint(touch.position);
            var normalizedPosition = _coordinateHelper.ScreenToWorldPointNormalizedToCamera(touch.position);
            var swipeData = _swipes[touch.fingerId];
            _swipes.Remove(touch.fingerId);
            var delta = position - swipeData.lastPosition;
            foreach(var subscriber in _swipeSubscribers)
            {
                subscriber.callback(new SwipeEventArgs(swipeData.origPosition,position, 
                    normalizedPosition, delta, SwipeStatus.End,touch.fingerId));
            }
        }


        private struct SwipeSubscriber
        {
            public Action<SwipeEventArgs> callback;
            public int instanceID;
        }

        private List<SwipeSubscriber> _swipeSubscribers = new List<SwipeSubscriber>();

        public void SubscribeSwipe(Action<SwipeEventArgs> callback, int instanceID)
        {
            _swipeSubscribers.Add(new SwipeSubscriber()
            {
                callback = callback,
                instanceID = instanceID
            });
        }

        public void UnsubscribeSwipe(int instanceID)
        {
            _swipeSubscribers.RemoveAll(o => o.instanceID == instanceID);
        }



    }
}