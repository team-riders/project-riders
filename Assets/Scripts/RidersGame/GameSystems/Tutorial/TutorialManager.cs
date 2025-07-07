using NUnit.Framework;
using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using RidersRuntime.Input;
using RidersRuntime.RaceManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject[] popUps;
        public int popUpIndex = 0;

        GameObject player;
        UnityEngine.InputSystem.PlayerInput m_playerInput;
        InputActionMap m_actionMap;
        string m_inputMapName = InputMap.Player;

        private bool clearedActionMap = false;

        private void Awake()
        {
        }

        void Start()
        {
            player = GameObject.Find("PlayerSet");
            m_playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            m_actionMap = m_playerInput.actions.FindActionMap(m_inputMapName);

        }

        // Update is called once per frame
        void Update()
        {
            if (!clearedActionMap)
            {
                foreach (InputAction action in m_actionMap.actions)
                {
                    action.Disable();
                }
                clearedActionMap = true;
            }

            for (int i = 0; i < popUps.Length; i++)
            {
                if (i == popUpIndex)
                {
                    popUps[i].SetActive(true);
                }
                else
                {
                    popUps[i].SetActive(false);
                }
            }

            // this part i feel like could be better but honestly i can't be assed thinking of a better way
            if (popUpIndex == 0)
            {
                m_actionMap.FindAction(ButtonNamesShort.Accelerate).Enable();
            }
            if (popUpIndex == 1)
            {
                m_actionMap.FindAction(ButtonNamesShort.Brake).Enable();
            }
            if (popUpIndex == 2)
            {
                m_actionMap.FindAction(ButtonNamesShort.TurnInput).Enable();
            }

            Debug.Log("popUpIndex: " + popUpIndex);
        }
        
    }
}
