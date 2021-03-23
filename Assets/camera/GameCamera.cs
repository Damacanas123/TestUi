using System;
using Slime.Helper;
using UnityEngine;

namespace camera
{
    public class GameCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            Util.DontDestroyOnLoad<GameCamera>(gameObject);
        }
    }
}
