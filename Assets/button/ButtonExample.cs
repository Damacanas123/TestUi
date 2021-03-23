using Slime.Helper.InputHelper;
using UnityEngine;

namespace button
{
    public class ButtonExample : MonoBehaviour
    {
        private Touchable _touchable;

        void Awake()
        {
            _touchable = GetComponent<Touchable>();
            _touchable.SetOnTouchDown((args) =>
            {
                Debug.Log($"Button touch down {args.position}");
                //return true to block other touch events
                return false;
            });
            _touchable.SetOnTouchUp((args) =>
            {
                Debug.Log($"Button touch up {args.position}");
            });
        }
    }
}
