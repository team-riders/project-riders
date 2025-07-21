using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using RidersRuntime.RaceManager;


namespace RidersRuntime.PauseSystem
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    public class pauseMenuScript : MonoBehaviour
    {
        public GameObject pauseGame;
        public bool isPaused = false;
        private static pauseMenuScript _instance;

        public static pauseMenuScript Instance { get { return _instance; } }
        private RaceHostController raceHostController;


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                ResetMap();
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            pauseGame.SetActive(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
        }

        public async void MainMenu()
        {
            await SceneLoader.PrepareScene(GameScene.MainMenu);
            pauseGame.SetActive(false);
            Time.timeScale = 1;
            raceHostController.ClearSession();
        }

        public async void ResetMap()
        {
            var selectionManager = FindFirstObjectByType<CharacterSelectionManager>();
            if (selectionManager != null)
            {
                CharacterSelectionManager.StoreSelectionsForReset(
                    selectionManager.GetCurrentSelections());
            }
            var meetController = FindFirstObjectByType<RaceMeetController>();
            if (meetController != null)
            {
                await meetController.ResetCurrentRace();
            }
            else
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                await SceneLoader.PrepareScene(currentSceneIndex);
            }
            var storedSelections = CharacterSelectionManager.GetStoredSelections();
            if (storedSelections != null)
            {
                var newManager = FindFirstObjectByType<CharacterSelectionManager>();
                if (newManager != null)
                {
                    newManager.RestoreSelections(storedSelections);
                }
            }
        }
    }
}
