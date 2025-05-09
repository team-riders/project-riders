using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.GameSystems
{
    public class MultiplayerControllerSystem : MonoBehaviour
    {
        // This is a placeholder for the multiplayer controller system.
        // You can add your multiplayer logic here.

        PlayerInputManager pim;

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            // Handle player joining logic here
            Debug.Log($"Player {playerInput.playerIndex} joined.");
        }

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            // Handle player leaving logic here
            Debug.Log($"Player {playerInput.playerIndex} left.");
        }
    }
}