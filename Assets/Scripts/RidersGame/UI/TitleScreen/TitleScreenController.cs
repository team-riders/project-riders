using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace RidersRuntime.UI
{
    public class TitleScreenController : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject titleUI;   // Assign TitleScreenUI here
        public GameObject menuUI;    // Assign MenuScreenUI here

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
        }

        void ShowMenu()
        {
            hasPressedKey = true;
            titleUI.SetActive(false);
            menuUI.SetActive(true);
        }
    }
}