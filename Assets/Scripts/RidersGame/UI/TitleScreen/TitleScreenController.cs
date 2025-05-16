using RidersRuntime.GameSystems;
using RidersRuntime.VehicleSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace RidersRuntime.UI
{
    public class TitleScreenController : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject titleUI;   // Assign TitleScreenUI here
        public GameObject menuUI;    // Assign MenuScreenUI here
        public GameObject menuDefaultButton;

        private float idleTimer = 0f;
        private float idleThreshold = 45f;
        private bool hasPressedKey = false;
        EventSystem mainNavigator;
        int mainPlayerIndex = -1;

        void Start()
        {
            titleUI.SetActive(true);
            menuUI.SetActive(false);

            SetupNewEntryUserInput();
            // ONLY CREATE THE FIRST PLAYER'S UI INPUT HERE FOR NOW
        }

        void Update()
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold && !hasPressedKey)
            {
                Debug.Log("Idle timeout reached - running placeholder function");
                idleTimer = 0f;
            }
        }

        void ShowMenu()
        {
            hasPressedKey = true;
            titleUI.SetActive(false);
            menuUI.SetActive(true);
            mainNavigator.SetSelectedGameObject(menuDefaultButton);
        }

        void SetupNewEntryUserInput()
        {
            MultiDeviceControllerSystem mdcs = FindFirstObjectByType<MultiDeviceControllerSystem>();
            // mdcs.onPlayerJoined += BindMainNavigatorControllerUIInput;
            mdcs.CanJoin(true);

            InputSystem.onAnyButtonPress.Call(currentAction =>
            {
                if (hasPressedKey) return;

                // This person becomes player 1
                InputUser? user = InputUser.FindUserPairedToDevice(currentAction.device);
                if (user == null)
                {
                    return;
                }
                BindMainNavigatorControllerUIInput(user.Value.index);

                ShowMenu();
                return;
            });

        }

        private void BindMainNavigatorControllerUIInput(int index)
        {
            if (mainPlayerIndex != -1 || mainNavigator != null)
            {
                // Already set
                Debug.Log("Main player already set");
                return;
            }
            mainPlayerIndex = index;
            // Set the default button for the menu
            EventSystem es = EventSystemSpawner.CreateNewPlayerEventSystem(index);
            mainNavigator = es;
        }
    }
}