using UnityEngine.SceneManagement;

namespace Slime.Helper.Scene
{
    public class FlowManager
    {
        public const int SplashIndex = 0;
        public const int GameIndex = 1;

        public static void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        
    }
}