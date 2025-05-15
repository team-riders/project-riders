using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.TextCore.Text;

namespace RidersRuntime.GameSystems
{
    public class MultiDeviceControllerSystem : MonoBehaviour
    {
        // This is a placeholder for the multiplayer controller system.
        // You can add your multiplayer logic here.

        PlayerInputManager pim;

        List<PlayerInput> players = new List<PlayerInput>();

        InputAction joinAction;

        List<InputDevice> inputDevices = new();

        void Awake()
        {
            joinAction = new InputAction(binding: "/*/<button>");
        }

        /// <summary>
        /// Based off https://discussions.unity.com/t/manual-local-multiplayer-using-inputdevices/884875/3
        /// </summary>
        /// <param name="context"></param>
        void OnJoinPressed(InputAction.CallbackContext context)
        {
            // THIS IS A MASSIVE TODO;
            return;
            InputDevice targetDevice = context.control.device;

            if (inputDevices.Contains(targetDevice))
            {
                return;
            }

            inputDevices.Add(targetDevice);

            // Check if we already at least 1 player
            if (InputUser.all.Count > 0)
            {
                InputUser player1 = InputUser.all[0];
                // Check if the player has a Keyboard&Mouse or Gamepad
                if (player1.pairedDevices.Count == 1)
                {
                    // Engage in special pairing here where we aim to have 2 control schemes on that player "for now"
                    InputDevice currentDevice = player1.pairedDevices[0];
                }
            }
        }

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            // Handle player joining logic here
            Debug.Log($"Player {playerInput.playerIndex} joined.");

            // Check if player 1 already exists and is keyboard only
            if (players.Count > 0 && players[0].currentControlScheme == "Keyboard&Mouse")
            {
                // If player 1 is keyboard only, add this device to player 1 then remove this player 
                InputDevice device = playerInput.devices[0];
                InputUser.PerformPairingWithDevice(device, players[0].user);
                Destroy(playerInput);
                return;
            }

            players.Add(playerInput);

            // If we're in the character selection scene, call join

            var CharSelect = FindFirstObjectByType<CharacterSelectionManager>();
            if (CharSelect != null)
            {
                // Create a new event system for the player that lives globally
                playerInput.GetComponent<RidersRuntime.Input.UnityInputWrapper>().SetCameraMode(false);
            }

            EventSystemSpawner.CreateNewPlayerEventSystem(playerInput.playerIndex);
        }

        // Every scene will need to have a different event system so we will need to continually create new event systems per screen
        // We COULD try do this as a static thingo that runs every new screen and is consistent

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            // Handle player leaving logic here
            Debug.Log($"Player {playerInput.playerIndex} left.");
            // We still need to keep the player incase of bug
            // Destroy(playerInput.gameObject);
        }

        public PlayerInput GetPlayerByPlayerIndex(int index)
        {
            if (!enabled) return FindFirstObjectByType<PlayerInput>();
            foreach (var player in players)
            {
                if (player.playerIndex == index)
                {
                    return player;
                }
            }
            Debug.LogWarning($"Player with index {index} not found.");
            return null;
        }

        public List<PlayerInput> GetAllPlayers()
        {
            return players;
        }

        public int GetPlayerCount()
        {
            if (!enabled) return 1;
            return players.Count;
        }
    }
}