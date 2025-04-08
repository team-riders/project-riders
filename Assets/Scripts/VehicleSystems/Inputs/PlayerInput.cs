using System.Collections.Generic;
using UnityEngine.InputSystem;


public class PlayerInput : BaseInput
{
    public InputActionAsset m_playerInput;
    public string m_inputMapName = "Player";
    ActorInputData currentFrameInputData = new();

    void Update()
    {
        currentFrameInputData = PollEveryRideInput();
    }

    ActorInputData PollEveryRideInput()
    {
        Dictionary<string, float> inputValues = new();

        foreach (InputAction action in m_playerInput.FindActionMap("Player").actions)
        {
            // Ignore pause
            if (action.name == "PauseButton")
                continue;

            if (action.type == InputActionType.Button)
            {
                inputValues.Add(action.name, action.IsPressed() ? 1 : 0);
            }
            else if (action.type == InputActionType.Value)
            {
                inputValues.Add(action.name, action.ReadValue<float>());
            }
        }

        // Convert to actor input data
        return new ActorInputData
        {
            Accelerate = inputValues["Accelerate"],
            Brake = inputValues["Brake"],
            TurnInput = inputValues["Horizontal"],

            Jump = inputValues["Jump"] > 0,
            StuntA = inputValues["Trick Button A"] > 0,
            StuntB = inputValues["Trick Button B"] > 0,
            StuntC = inputValues["Trick Button C"] > 0,

            Drift = inputValues["Drift"] > 0,
            BoostRam = inputValues["Boost/Ram"] > 0,
        };
    }

    public override ActorInputData GrabCurrentFrameInputs() => currentFrameInputData;
}
