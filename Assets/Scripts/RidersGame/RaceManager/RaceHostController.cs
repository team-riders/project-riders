using RidersRuntime.Data;
using RidersRuntime.RaceManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RidersRuntime.GameSystems
{
    public class RaceHostController : MonoBehaviour
    {
        // This class manages the dispatching of the race and awaits the character selections

        RaceMeetConfiguration sessionMeet;

        RaceMeetController sessionRaceMeetController;

        static RaceHostController instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static RaceHostController GetInstance()
        {
            if (instance == null)
            {
                Debug.LogError("RaceHostController instance is null.");
                return null;
            }
            return instance;
        }

        public void ClearSession()
        {
            if (sessionRaceMeetController != null)
            {
                Destroy(sessionRaceMeetController.gameObject);
                sessionRaceMeetController = null;
            }
            sessionMeet = null;
        }

        public async void RequestRaceSetup(RaceMeetConfiguration meetConfiguration)
        {
            if (sessionMeet != null)
            {
                Debug.LogError("A race is already in progress.");
                Debug.LogError(sessionMeet);
                return;
            }
            sessionMeet = meetConfiguration;
            // This method will be called when a race type has been selected
            // For the time being there will be just a single player

            if (GetComponent<MultiDeviceControllerSystem>().GetPlayerCount() == 0)
            {
                Debug.LogError("Please ensure that there is at least one player in the game");
                return;
            }

            // ------ SCENE TRANSITION -------
            // await the scene transition to complete

            // StartCoroutine(LoadNewSceneAsync((int)GameScene.CharacterSelect));
            await SceneLoader.PrepareScene(GameScene.CharacterSelect, onLoad: PrepareCharacterSelection);
        }

        private void PrepareCharacterSelection()
        {
            CharacterSelectionManager selector = FindFirstObjectByType<CharacterSelectionManager>();
            selector.GrabHostInformation(sessionMeet, OnRaceMeetSetupComplete);
        }

        public async void OnRaceMeetSetupComplete(List<RiderSelection> selectedRiders)
        {
            // This method will be called when the character selection is complete
            // Check with the number of players to see if we need to add any AI riders

            // Check if we already have a race session
            if (sessionRaceMeetController != null)
            {
                Debug.LogError("A race session has already been started.");
                return;
            }

            int playerCount = selectedRiders.Count;
            int aiCount = sessionMeet.NumberOfRacers - playerCount;

            List<RiderSelection> allRiders = new(selectedRiders);

            for (int i = 0; i < aiCount; i++)
            {
                RiderSelection aiRider = CreateUniqueBotRider(allRiders);
                allRiders.Add(aiRider);
            }

            GameObject obj = new GameObject("RaceMeetController");
            obj.transform.SetParent(transform);
            sessionRaceMeetController = obj.AddComponent<RaceMeetController>();

            sessionRaceMeetController.AssignFields(sessionMeet, allRiders, OnRaceMeetComplete);

            await sessionRaceMeetController.StartNewMeetSession();
        }

        // used as argument for AssignFields, becomes assigned to onRaceComplete in RaceMeetController.cs
        public void OnRaceMeetComplete(object obj)
        {
            //int index = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/prod/TitleScreen.unity");
            //await SceneLoader.PrepareScene(index);
        }

        public static RiderSelection CreateUniqueBotRider(List<RiderSelection> alreadySelectedRiders)
        {
            // This method will create a unique bot rider that is not already selected by the players

            // Get a random rider from the list of available riders

            RiderConfig[] obj = Resources.LoadAll<RiderConfig>(Paths.Riders);
            // Why bother choosing random, just pick the first one LMAO
            RiderConfig randomRider = obj[0];
            // If the rider is not already selected, return it as a new RiderSelection
            return new RiderSelection
            {
                rider = randomRider,
                vehicleType = randomRider.racerInformation.defaultVehicleType,
                isPlayer = false
            };
        }
    }
}