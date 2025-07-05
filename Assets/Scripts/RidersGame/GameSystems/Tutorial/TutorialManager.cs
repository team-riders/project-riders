using RidersRuntime.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject[] popUps;
        private int popUpIndex;

        GameObject player;
        UnityEngine.InputSystem.PlayerInput m_playerInput;
        InputActionMap m_actionMap;
        string m_inputMapName = InputMap.Player;

        void Start()
        {
            player = GameObject.Find("PlayerSet");
            m_playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            m_actionMap = m_playerInput.actions.FindActionMap(m_inputMapName);

            foreach (InputAction action in m_actionMap.actions)
            {
                action.Disable();
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                if (i == popUpIndex)
                {
                    popUps[popUpIndex].gameObject.SetActive(true);
                }
                else
                {
                    popUps[popUpIndex].gameObject.SetActive(false);
                }
            }

            // this part i feel like could be better but honestly i can't be assed thinking of a better way
            if (popUpIndex == 0)
            {
                m_actionMap.FindAction(ButtonNamesShort.Accelerate).Enable();
            }
        }
    }
}
