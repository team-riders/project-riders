using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.GameSystems
{
    public class MultiDeviceControllerSystem : MonoBehaviour
    {
        // This is a placeholder for the multiplayer controller system.
        // You can add your multiplayer logic here.

        PlayerInputManager pim;

        List<PlayerInput> players = new List<PlayerInput>();

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            // Handle player joining logic here
            Debug.Log($"Player {playerInput.playerIndex} joined.");
            players.Add(playerInput);
        }

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            // Handle player leaving logic here
            Debug.Log($"Player {playerInput.playerIndex} left.");
            // We still need to keep the player incase of bug
            // Destroy(playerInput.gameObject);
        }

        public PlayerInput GetPlayerByPlayerIndex(int index)
        {
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
            return players.Count;
        }
    }
}