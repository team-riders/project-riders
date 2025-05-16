using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

        void Start()
        {
            titleUI.SetActive(true);
            menuUI.SetActive(false);

            InputSystem.onAnyButtonPress.Call(currentAction =>
            {
                if (hasPressedKey) return;

                ShowMenu();
                return;
            });
        }

        void Update()
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold && !hasPressedKey)
            {
                Debug.Log("Idle timeout reached - running placeholder function");
                idleTimer = 0f;
            }

            if (EventSystem.current != null)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.firstSelectedGameObject = menuDefaultButton;
                }
            }
        }

        void ShowMenu()
        {
            hasPressedKey = true;
            titleUI.SetActive(false);
            menuUI.SetActive(true);
        }

        public void SetDefaultButton(Button button)
        {
            if (menuUI.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                button.Select();
            }
        }
    }
}