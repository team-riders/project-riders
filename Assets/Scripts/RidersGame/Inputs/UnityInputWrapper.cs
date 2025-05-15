using RidersRuntime.GameSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace RidersRuntime.Input
{
    public class UnityInputWrapper : MonoBehaviour
    {
        bool isGameplayCamera = false;
        UnityEngine.InputSystem.PlayerInput input;
        public void Start()
        {
            input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            // ALL PLAYERS WILL BE PERSISTENT THROUGHOUT THE GAME
            DontDestroyOnLoad(gameObject);
            SetCameraMode(false);
            // Check if we have an event system bound
            if (input.uiInputModule == null)
            {
                SetupUIInputs();
            }
        }

        public void Update()
        {
            // If there's no event system, make one
            if (input.uiInputModule == null)
            {
                SetupUIInputs();
            }
        }

        public void SetCameraReference(Camera camera)
        {
            input.camera = camera;
        }

        public void SetCameraMode(bool isGameplayCamera)
        {
            this.isGameplayCamera = isGameplayCamera;
            if (input.camera == null)
            {
                Debug.LogWarning("Camera reference is not set in PlayerInput.");
                return;
            }
            if (input == null)
            {
                input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            }
            if (isGameplayCamera)
            {
                input.camera.enabled = true;
            }
            else
            {
                // Set the camera to character selection mode
                input.camera.enabled = false;
                Debug.Log("Character selection camera mode activated.");
            }
        }

        public void OnDeviceLost(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            Debug.Log($"Device lost: {playerInput}");
        }

        public void OnDeviceRegained(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            Debug.Log($"Device regained: {playerInput}");
        }

        public void SetupUIInputs()
        {
            EventSystemSpawner.CreateNewPlayerEventSystem(input.playerIndex);
        }
    }
}