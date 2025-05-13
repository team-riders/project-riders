using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace RidersRuntime.Input
{
    public class UnityInputWrapper : MonoBehaviour
    {
        public void Start()
        {
            // ALL PLAYERS WILL BE PERSISTENT THROUGHOUT THE GAME
            DontDestroyOnLoad(gameObject);
        }
        public void OnDeviceLost(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            Debug.Log($"Device lost: {playerInput}");
        }

        public void OnDeviceRegained(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            Debug.Log($"Device regained: {playerInput}");
        }
    }
}