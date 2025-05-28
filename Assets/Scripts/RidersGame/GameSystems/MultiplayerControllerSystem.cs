using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace RidersRuntime.GameSystems
{
    public class MultiDeviceControllerSystem : MonoBehaviour
    {
        // This is a placeholder for the multiplayer controller system.
        // You can add your multiplayer logic here.

        PlayerInputManager pim;
        List<PlayerInput> players = new();
        InputAction joinAction;
        List<InputDevice> inputDevices = new();

        [SerializeField]
        bool SinglePlayerMode = false;
        [SerializeField]
        bool Player1SecondaryControls = true;


        public Action<int> onPlayerJoined;

        void Awake()
        {
            if (FindObjectsByType<MultiDeviceControllerSystem>(FindObjectsSortMode.None).Length > 1)
            {
                Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(this);
            }
        }

        void Start()
        {
            pim = GetComponent<PlayerInputManager>();
            joinAction = new InputAction(binding: "/*/<button>");
            joinAction.started += OnJoinPressed;
            joinAction.Enable();
        }

        /// <summary>
        /// Based off https://discussions.unity.com/t/manual-local-multiplayer-using-inputdevices/884875/3
        /// </summary>
        /// <param name="context"></param>
        void OnJoinPressed(InputAction.CallbackContext context)
        {
            InputDevice targetDevice = context.control.device;

            if (inputDevices.Contains(targetDevice))
            {
                int index = GetPlayerIndexByDevicePaired(targetDevice);
                if (index != -1)
                {
                    Debug.Log($"Device {targetDevice.displayName} already paired to player {index}");
                    InputUser.all[index].ActivateControlScheme(GetControlSchemeByDeviceName(targetDevice.displayName));
                    return;
                }
                return;
            }

            // Check if we already at least 1 player
            if (InputUser.all.Count > 0)
            {
                InputUser player1 = InputUser.all[0];
                // Check if the player has a Keyboard&Mouse or Gamepad
                if ((!SinglePlayerMode && Player1SecondaryControls && player1.pairedDevices.Count == 1) || SinglePlayerMode)
                {
                    players[player1.index].neverAutoSwitchControlSchemes = false;
                    inputDevices.Add(targetDevice);
                    InputUser.PerformPairingWithDevice(targetDevice, player1);
                    player1.ActivateControlScheme(GetControlSchemeByDeviceName(targetDevice.displayName));
                    Debug.Log($"Bound secondary controls {targetDevice.displayName} to player {player1.index}");
                    return;
                }
            }

            inputDevices.Add(targetDevice);
            PlayerInput player = pim.JoinPlayer(players.Count, pairWithDevice: targetDevice);
            if (player != null)
            {
                players.Add(player);
                Debug.Log($"Player {player.playerIndex} joined via MultiDeviceControllerSystem.");
                onPlayerJoined?.Invoke(player.playerIndex);
            }
        }

        public PlayerInput GetPlayerInputByEventSystem(EventSystem eventSystem)
        {
            if (!enabled) return FindFirstObjectByType<PlayerInput>();
            foreach (var player in players)
            {
                if (player.GetComponent<EventSystem>() == eventSystem)
                {
                    return player;
                }
            }
            Debug.LogWarning($"Player with EventSystem {eventSystem} not found.");
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

        public int GetPlayerIndexByDevicePaired(InputDevice device)
        {
            if (!enabled) return 0;
            foreach (var player in players)
            {
                if (player.devices.Contains(device))
                {
                    return player.playerIndex;
                }
            }
            Debug.LogWarning($"Player with device {device} not found.");
            return -1;
        }

        public string GetControlSchemeByDeviceName(string deviceName)
        {
            if (!enabled) return "Keyboard&Mouse";
            if (deviceName.Contains("Gamepad") || deviceName.Contains("Controller"))
            {
                return "Gamepad";
            }
            else if (deviceName.Contains("Keyboard") || deviceName.Contains("Mouse"))
            {
                return "Keyboard&Mouse";
            }
            return "Keyboard&Mouse";
        }

        public void CanJoin(bool canJoin)
        {
            return;
            if (canJoin)
            {
                joinAction.Enable();
            }
            else
            {
                joinAction.Disable();
            }
        }
    }
}