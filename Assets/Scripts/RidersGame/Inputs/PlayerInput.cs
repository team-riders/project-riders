using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.Input
{
    public class PlayerInput : BaseInput
    {
        UnityEngine.InputSystem.PlayerInput m_playerInput;
        string m_inputMapName = InputMap.Player;
        ActorInputData currentFrameInputData = new();

        void Start()
        {
            // TODO: Need to move a shit ton out of start and move to a separate function
            // TODO: If it's not present here, we should check the parent as well. 
            // The board may not necessarily have the Player Input component
            if (m_playerInput == null)
            {
                UnityEngine.InputSystem.PlayerInput playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
                playerInput = playerInput != null ? playerInput : GetComponentInParent<UnityEngine.InputSystem.PlayerInput>();

                if (playerInput == null)
                {
                    Debug.LogError("PlayerInput component not found in the GameObject or its parents.");
                    return;
                }

                m_playerInput = playerInput;
            }
        }

        public void BindPlayerInput(UnityEngine.InputSystem.PlayerInput inputComponent)
        {
            m_playerInput = inputComponent;
        }

        public UnityEngine.InputSystem.PlayerInput GetPlayerInputComponent()
        {
            return m_playerInput;
        }

        void Update()
        {
            currentFrameInputData = PollEveryRideInput();
        }

        ActorInputData PollEveryRideInput()
        {
            Dictionary<string, float> inputValues = new();

            foreach (InputAction action in m_playerInput.actions.FindActionMap(m_inputMapName).actions)
            {
                // Ignore pause
                if (action.name == ButtonNamesShort.PauseButton)
                    continue;

                if (action.type == InputActionType.Button)
                {
                    // Special handle for Jump
                    if (action.name == ButtonNamesShort.Jump)
                    {
                        inputValues[ButtonNamesShort.Jump] = action.WasReleasedThisFrame() ? 1 : 0;
                        inputValues[InputNameSpecial.JumpHoldDuration] = (true) switch
                        {
                            true when action.WasPressedThisFrame() => 0,
                            true when action.WasReleasedThisFrame() => currentFrameInputData.JumpHoldDuration,
                            true when action.IsPressed() => currentFrameInputData.JumpHoldDuration + 1,
                            _ => 0
                        };
                    }
                    else
                    {
                        inputValues.Add(action.name, action.IsPressed() ? 1 : 0);
                    }
                }
                else if (action.type == InputActionType.Value)
                {
                    inputValues.Add(action.name, action.ReadValue<float>());
                }
            }

            // Convert to actor input data
            return new ActorInputData
            {
                Accelerate = inputValues[ButtonNamesShort.Accelerate],
                Brake = inputValues[ButtonNamesShort.Brake],
                TurnInput = inputValues[ButtonNamesShort.TurnInput],

                Jump = inputValues[ButtonNamesShort.Jump] > 0,
                JumpHoldDuration = inputValues[InputNameSpecial.JumpHoldDuration],
                StuntA = inputValues[StuntButtonNamesShort.StuntA] > 0,
                StuntB = inputValues[StuntButtonNamesShort.StuntB] > 0,
                StuntC = inputValues[StuntButtonNamesShort.StuntC] > 0,

                Drift = inputValues[ButtonNamesShort.Drift] > 0,
                BoostRam = inputValues[ButtonNamesShort.BoostRam] > 0,
            };
        }

        public override ActorInputData GrabCurrentFrameInputs() => currentFrameInputData;
    }

}