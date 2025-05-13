using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RidersRuntime.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
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

        [SerializeField]
        public Button defaultButton;

        [SerializeField]
        private Button startButton;

        void Start()
        {
            // Check if god exists
            if (!FindFirstObjectByType<RaceHostController>())
            {
                // Manually assign stuff for now
                debugMode = true;


                BindAllPlayersUI();
            }
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
            MultiplayerEventSystem[] eventSystem = FindObjectsByType<MultiplayerEventSystem>(FindObjectsSortMode.InstanceID);

            foreach (var eventSys in eventSystem)
            {
                Debug.Log("Binding player UI input for " + eventSys.gameObject.name);
                // We need to destroy and set it up again
                BindNewPlayerUIInput(eventSys);
            }
            ValidateForm();
        }

        public void BindNewPlayerUIInput(EventSystem eventSystem)
        {
            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(defaultButton.gameObject);
                eventSystem.enabled = true;

                int caller = eventSystem.GetComponentInParent<UnityEngine.InputSystem.PlayerInput>().playerIndex;

                playerRiderSelection.Add(caller, new RiderSelection
                {
                    rider = null,
                    vehicleType = VehicleType.None,
                    isPlayer = true
                });
            }
        }

        public async void ReturnToMainMenu()
        {
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
                    Debug.Log("Player " + selection.Key + " hasn't made a selection");
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

        private UnityEngine.InputSystem.PlayerInput GetPlayerInput()
        {
            return EventSystem.current.GetComponentInParent<UnityEngine.InputSystem.PlayerInput>();
        }

        public void OnCharacterSelected(RiderConfig racer)
        {
            // This method will be called when a character is selected
            // Nightmare fuel to handle who is selecting what
            int caller = GetPlayerInput().playerIndex;
            Debug.Log("Character selected: " + caller + "\n" + racer.name);

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

            StartCoroutine(ShowCharacterSelected(caller));

            ValidateForm();
        }

        IEnumerator ShowCharacterSelected(int playerIndex)
        {
            if (playerRiderSelection.ContainsKey(playerIndex))
            {
                RiderSelection selection = playerRiderSelection[playerIndex];
                GameObject charPrefab = selection.rider.characterModelPrefab;

                GameObject characterPosition = characterPositions[playerIndex];

                if (characterPosition.transform.childCount > 0)
                {
                    // Kill all children
                    foreach (Transform child in characterPosition.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    yield return new WaitForSeconds(0.3f);
                }

                GameObject newCharacter = Instantiate(charPrefab, characterPosition.transform);
            }
            else
            {
                Debug.Log("No character selected for player " + playerIndex);
            }
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

        public async void OnExitButton()
        {
            await SceneLoader.PrepareScene(GameScene.MainMenu);
        }
    }
}