using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

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
            if (action.name == Values.ButtonNamesShort.PauseButton)
                continue;

            if (action.type == InputActionType.Button)
            {
                // Special handle for Jump
                if (action.name == Values.ButtonNamesShort.Jump)
                {
                    if (action.WasPressedThisFrame())
                    {
                        inputValues[Values.ButtonNamesShort.Jump] = 0;
                        inputValues[Values.InputNameSpecial.JumpHoldDuration] = 0;
                    }
                    else if (action.WasReleasedThisFrame())
                    {
                        inputValues[Values.ButtonNamesShort.Jump] = 1;
                        inputValues[Values.InputNameSpecial.JumpHoldDuration] = currentFrameInputData.JumpHoldDuration;
                    }
                    else if (action.IsPressed())
                    {
                        inputValues[Values.ButtonNamesShort.Jump] = 0;
                        inputValues[Values.InputNameSpecial.JumpHoldDuration] = currentFrameInputData.JumpHoldDuration + 1;
                    }
                    else
                    {
                        inputValues[Values.ButtonNamesShort.Jump] = 0;
                        inputValues[Values.InputNameSpecial.JumpHoldDuration] = 0;
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
            Accelerate = inputValues[Values.ButtonNamesShort.Accelerate],
            Brake = inputValues[Values.ButtonNamesShort.Brake],
            TurnInput = inputValues[Values.ButtonNamesShort.TurnInput],

            Jump = inputValues[Values.ButtonNamesShort.Jump] > 0,
            JumpHoldDuration = inputValues[Values.InputNameSpecial.JumpHoldDuration],
            StuntA = inputValues[Values.StuntButtonNamesShort.StuntA] > 0,
            StuntB = inputValues[Values.StuntButtonNamesShort.StuntB] > 0,
            StuntC = inputValues[Values.StuntButtonNamesShort.StuntC] > 0,

            Drift = inputValues[Values.ButtonNamesShort.Drift] > 0,
            BoostRam = inputValues[Values.ButtonNamesShort.BoostRam] > 0,
        };
    }

    public override ActorInputData GrabCurrentFrameInputs() => currentFrameInputData;
}
