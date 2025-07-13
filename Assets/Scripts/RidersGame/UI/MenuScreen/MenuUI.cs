using UnityEngine;
using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace RidersRuntime.UI
{
    public class MenuUI : MonoBehaviour
    {
        private RaceHostController hostController;

        private RaceMeetConfiguration selectedMeet;
        private bool isTutorialMode = false;

        void Start()
        {
            hostController = FindFirstObjectByType<RaceHostController>();
        }
        public async Task LoadMainMenu()
        {
            await SceneLoader.PrepareScene(GameScene.MainMenu);
        }

        public void setSelectedMeet(RaceMeetConfiguration meet)
        {
            selectedMeet = meet;
            isTutorialMode = false;
        }

        public void SetTutorialMode()
        {
            selectedMeet = null;
            isTutorialMode = true;
        }

        public void LoadCharacterSelect()
        {
            if (isTutorialMode)
            {
                LoadTutorial();
                return;
            }
            if (selectedMeet == null)
            {
                Debug.LogError("No meet selected");
                return;
            }
            hostController.RequestRaceSetup(selectedMeet);
        }
		
		public void QuitGame()
		{
			Application.Quit();
			Debug.Log("Congrats, you closed the game. Yay!");
		}
		
        public async void LoadTutorial()
        {
            int index = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/environments/map_rooftop.unity");
            await SceneLoader.PrepareScene(index);
        }
    }
}