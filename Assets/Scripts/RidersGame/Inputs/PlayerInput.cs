using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.Input
{
    [RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
    public class PlayerInput : BaseInput
    {
        InputActionAsset m_playerInput;
        public string m_inputMapName = "Player";
        ActorInputData currentFrameInputData = new();

        void Start()
        {
            m_playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>().actions;
        }
        void Update()
        {
            currentFrameInputData = PollEveryRideInput();
        }

        ActorInputData PollEveryRideInput()
        {
            Dictionary<string, float> inputValues = new();

            foreach (InputAction action in m_playerInput.FindActionMap(m_inputMapName).actions)
            {
                // Ignore pause
                if (action.name == ButtonNamesShort.PauseButton)
                    continue;

                if (action.type == InputActionType.Button)
                {
                    // Special handle for Jump
                    if (action.name == ButtonNamesShort.Jump)
                    {
                        if (action.WasPressedThisFrame())
                        {
                            inputValues[ButtonNamesShort.Jump] = 0;
                            inputValues[InputNameSpecial.JumpHoldDuration] = 0;
                        }
                        else if (action.WasReleasedThisFrame())
                        {
                            inputValues[ButtonNamesShort.Jump] = 1;
                            inputValues[InputNameSpecial.JumpHoldDuration] = currentFrameInputData.JumpHoldDuration;
                        }
                        else if (action.IsPressed())
                        {
                            inputValues[ButtonNamesShort.Jump] = 0;
                            inputValues[InputNameSpecial.JumpHoldDuration] = currentFrameInputData.JumpHoldDuration + 1;
                            UnityEngine.Debug.Log($"JumpHoldDuration: {currentFrameInputData.JumpHoldDuration}");
                        }
                        else
                        {
                            inputValues[ButtonNamesShort.Jump] = 0;
                            inputValues[InputNameSpecial.JumpHoldDuration] = 0;
                        }
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