using System.Collections;
using UnityEngine;

namespace Slime.Helper.Animation
{
    public class AnimationHelper
    {
        private static float[] ShakeArray = {5, 10, 15, 20, 25, 20, 15, 10, 5, 0, -5, -10, -15, -20, -25, -20, -15, -10, -5, 0};
        
        public static IEnumerator ShakeAnimation(GameObject gameObject,int shakeCount)
        {
            var eulerAngles = gameObject.transform.eulerAngles;

            for (int j = 0; j < shakeCount; j++)
            {
                for (int i = 0; i < ShakeArray.Length; i++)
                {
                    gameObject.transform.eulerAngles =
                        new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z + ShakeArray[i]);
                    yield return new WaitForSeconds(0.016f);
                }    
            }

            yield return null;
        }
    }
}