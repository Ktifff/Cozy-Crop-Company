using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class GameLoader : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene(1);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}