using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RidersRuntime.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

namespace RidersRuntime.GameSystems
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        Dictionary<int, RiderSelection> playerRiderSelection = new();

        RaceMeetConfiguration sessionMeet;

        public List<GameObject> characterPositions = new();

        Action<List<RiderSelection>> onCharacterSelectionComplete;

        // Triggered if this scene is loaded without god
        public bool debugMode = false;
        public float charAppearanceDelay = 0.1f;

        [SerializeField]
        public Button defaultButton;

        [SerializeField]
        private Button startButton;

        Dictionary<int, IEnumerator> charIndexOperationQueue = new();
        MultiDeviceControllerSystem multiDeviceController;
        void Start()
        {
            // Check if god exists
            if (!FindFirstObjectByType<RaceHostController>())
            {
                // Manually assign stuff for now
                debugMode = true;

                BindAllPlayersUI();
            }
            multiDeviceController.CanJoin(true);
        }

        public void GenerateCharacterSelection()
        {
            // Here we need to do the following
            // 1. Load all the characters that we can select
            // 2. Arrange them in a grid based on the following parameters
            // a. Each row represents a faction
            // 3. Bind buttons to the characters
            // 4. Bind the character details to the buttons to generate the sprites, behaviours, data, etc.
            // 5. Bind UI Components to all the respective players
        }

        public void BindAllPlayersUI()
        {
            multiDeviceController = FindFirstObjectByType<MultiDeviceControllerSystem>();
            if (multiDeviceController != null)
            {
                multiDeviceController.onPlayerJoined += BindNewPlayerUIInput;
            }

            foreach (var user in InputUser.all)
            {
                Debug.Log("Setting up UI for player " + user.index);
                BindNewPlayerUIInput(user.index);
            }

            ValidateForm();
        }

        public void BindNewPlayerUIInput(int index)
        {
            EventSystem eventSystem = EventSystemSpawner.CreateNewPlayerEventSystem(index);
            if (eventSystem != null)
            {
                // Find the base event system
                eventSystem.firstSelectedGameObject = defaultButton.gameObject;
                eventSystem.enabled = true;

                playerRiderSelection.Add(index, new RiderSelection
                {
                    rider = null,
                    vehicleType = VehicleType.None,
                    isPlayer = true
                });
            }
        }

        public async void ReturnToMainMenu()
        {
            multiDeviceController.onPlayerJoined -= BindNewPlayerUIInput;
            await SceneLoader.PrepareScene(GameScene.MainMenu); ;
        }

        private bool ValidateForm()
        {
            bool isValid = true;
            // If there's no player, hide it

            if (playerRiderSelection.Count == 0)
            {
                Debug.Log("No players in the game");
                isValid = false;
            }

            foreach (var selection in playerRiderSelection)
            {
                if (isValid == false)
                {
                    break;
                }
                // Check if the player exists
                if (false)
                {
                    Debug.Log("Player Doesn't exist or hasn't made a selection");
                    isValid = false;
                    break;
                }

                // Check if null
                if (selection.Value.rider == null)
                {
                    // Debug.Log("Player " + selection.Key + " hasn't made a selection");
                    isValid = false;
                    break;
                }

                if (selection.Value.rider == null)
                {
                    Debug.Log("Player " + selection.Key + " hasn't made a character selection");
                    isValid = false;
                    break;
                }

                if (selection.Value.vehicleType == VehicleType.None)
                {
                    Debug.Log("Player " + selection.Key + " hasn't made a vehicle selection");
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                // Show the start button
                startButton.gameObject.SetActive(true);
            }
            else
            {
                // Hide the start button
                startButton.gameObject.SetActive(false);
            }

            return isValid;
        }

        public void OnCharacterSelected(RiderConfig racer)
        {
            int caller = -1;

            foreach (var user in PlayerInput.all)
            {
                if (user.uiInputModule.GetComponent<EventSystem>() == EventSystem.current)
                {
                    caller = user.playerIndex;
                    break;
                }
            }


            if (playerRiderSelection.ContainsKey(caller))
            {
                playerRiderSelection[caller].rider = racer;
                playerRiderSelection[caller].vehicleType = racer.racerInformation.defaultVehicleType;
                playerRiderSelection[caller].isPlayer = true;
            }
            else
            {
                RiderSelection newSelection = new RiderSelection
                {
                    rider = racer,
                    vehicleType = racer.racerInformation.defaultVehicleType,
                    isPlayer = true
                };
                playerRiderSelection.Add(caller, newSelection);
            }

            IEnumerator coroutine = ShowCharacterSelected(caller);
            if (charIndexOperationQueue.ContainsKey(caller))
            {
                StopCoroutine(charIndexOperationQueue[caller]);
            }
            else
            {
                charIndexOperationQueue.Add(caller, coroutine);
            }

            StartCoroutine(coroutine);

            ValidateForm();
        }

        IEnumerator ShowCharacterSelected(int playerIndex)
        {
            GameObject characterPosition = characterPositions[playerIndex];
            if (characterPosition.transform.childCount > 0)
            {
                foreach (Transform child in characterPosition.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (playerRiderSelection.ContainsKey(playerIndex))
            {
                RiderSelection selection = playerRiderSelection[playerIndex];
                GameObject charPrefab = selection.rider.characterModelPrefab;

                yield return new WaitForSeconds(charAppearanceDelay);

                Instantiate(charPrefab, characterPosition.transform);
            }
            else
            {
                Debug.Log("No character selected for player " + playerIndex);
            }

            charIndexOperationQueue.Remove(playerIndex);

        }

        public void GrabHostInformation(RaceMeetConfiguration meetConfiguration, Action<List<RiderSelection>> callback)
        {
            onCharacterSelectionComplete = callback;
            sessionMeet = meetConfiguration;
            BindAllPlayersUI();
        }

        public void DispatchToMaster()
        {
            if (debugMode)
            {
                DebugCharacterSelect();
            }
            else
            {
                onCharacterSelectionComplete?.Invoke(playerRiderSelection.Values.ToList());
            }
        }

        void DebugCharacterSelect()
        {
            if (debugMode)
            {
                // Log the current selection
                foreach (var rider in playerRiderSelection.Values)
                {
                    Debug.Log($"Rider: {rider.rider.name}, Vehicle: {rider.vehicleType}, IsPlayer: {rider.isPlayer}");
                }
            }
        }
    }
}