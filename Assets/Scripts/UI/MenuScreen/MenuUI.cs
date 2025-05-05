using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneLoader.LoadScene(Scene.MainMenu);
    }
}
