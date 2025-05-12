using UnityEngine;
using RidersRuntime.Data;
using UnityEngine.SceneManagement;

namespace RidersRuntime.UI
{
    public class MenuUI : MonoBehaviour
    {
        public void LoadMainMenu()
        {
            SceneLoader.LoadScene(GameScene.MainMenu);
        }
    }
}