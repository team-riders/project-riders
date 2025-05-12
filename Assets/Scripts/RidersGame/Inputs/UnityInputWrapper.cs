using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace RidersRuntime.Input
{
    public class UnityInputWrapper : MonoBehaviour
    {
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