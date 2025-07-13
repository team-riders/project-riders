using NUnit.Framework;
using RidersRuntime.Data;
using RidersRuntime.GameSystems;
using RidersRuntime.Input;
using RidersRuntime.RaceManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RidersRuntime
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject[] popUps;
        public int popUpIndex = 0;

        public GameObject player;
        UnityEngine.InputSystem.PlayerInput m_playerInput;
        InputActionMap m_actionMap;
        string m_inputMapName = InputMap.Player;

        void Start()
        {
            m_playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            m_actionMap = m_playerInput.actions.FindActionMap(m_inputMapName);
        }

        // Update is called once per frame
        void Update()
        {
            // disables unneeded inputs until appropriate steps
            if (popUpIndex == 0)
            {
                m_actionMap.FindAction(ButtonNamesShort.Brake).Disable();
                m_actionMap.FindAction(ButtonNamesShort.TurnInput).Disable();
                m_actionMap.FindAction(ButtonNamesShort.Jump).Disable();
                m_actionMap.FindAction(ButtonNamesShort.BoostRam).Disable();
            }

            // disables objects not part of tutorial step, enables objects part of step
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
            // enables appropriate inputs at step
            if (popUpIndex >= 0)
            {
                m_actionMap.FindAction(ButtonNamesShort.Accelerate).Enable();
            }
            if (popUpIndex >= 1)
            {
                m_actionMap.FindAction(ButtonNamesShort.Brake).Enable();
            }
            if (popUpIndex >= 2)
            {
                m_actionMap.FindAction(ButtonNamesShort.TurnInput).Enable();
            }
            if (popUpIndex >= 3)
            {
                m_actionMap.FindAction(ButtonNamesShort.Jump).Enable();
            }
            if (popUpIndex >= 5)
            {
                m_actionMap.FindAction(ButtonNamesShort.BoostRam).Enable();
            }
            if (popUpIndex >= 7)
            {
                LoadMainMenu();
            }

        }

        public async void LoadMainMenu()
        {
            int index = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/prod/TitleScreen.unity");
            await SceneLoader.PrepareScene(index);
        }

    }
}
