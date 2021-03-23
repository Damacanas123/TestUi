using Slime.Helper.InputHelper;
using UnityEngine;

namespace swipe_container
{
    public class SwipeContainer : MonoBehaviour
    {
        private Touchable _touchable;

        void Awake()
        {
            _touchable = GetComponent<Touchable>();
            _touchable.SetOnSwipe((args) =>
            {
                //inspect swipe args fields for more information, you can always ask Barkin
                Debug.Log($"On swipe {args.lastPosition}");
                foreach (Transform child in transform)
                {
                    child.transform.position += new Vector3(args.delta.x,args.delta.y,0);
                }
            });
        }
    }
}
