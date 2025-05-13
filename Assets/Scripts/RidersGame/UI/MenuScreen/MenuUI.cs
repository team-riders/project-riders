using UnityEngine;
using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using System.Threading.Tasks;

namespace RidersRuntime.UI
{
    public class MenuUI : MonoBehaviour
    {
        private RaceHostController hostController;

        private RaceMeetConfiguration selectedMeet;

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
        }

        public void LoadCharacterSelect()
        {
            if (selectedMeet == null)
            {
                Debug.LogError("No meet selected");
                return;
            }
            hostController.RequestRaceSetup(selectedMeet);
        }
    }
}