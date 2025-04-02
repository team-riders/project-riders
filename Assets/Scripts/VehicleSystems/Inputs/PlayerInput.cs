using UnityEngine.InputSystem;

public class PlayerInput : BaseInput
{
    public string TurnInputName = "Horizontal";
    public string AccelerateButtonName = "Accelerate";
    public string BrakeButtonName = "Brake";
    public string JumpButtonName = "Jump";

    public string TrickButtonAName = "Trick Button A";
    public string TrickButtonBName = "Trick Button B";
    public string TrickButtonCName = "Trick Button C";

    public string PauseButtonName = "Pause";

    public string DriftButtonName = "Drift";
    public string BoostRamButtonName = "Boost/Ram";

    public override InputData GenerateInput()
    {
        return new InputData
        {
            Accelerate = InputSystem.actions.FindAction(AccelerateButtonName),
            Brake = InputSystem.actions.FindAction(BrakeButtonName),
            TurnInput = InputSystem.actions.FindAction(TurnInputName),
            Jump = InputSystem.actions.FindAction(JumpButtonName),

            TrickButtonA = InputSystem.actions.FindAction(TrickButtonAName),
            TrickButtonB = InputSystem.actions.FindAction(TrickButtonBName),
            TrickButtonC = InputSystem.actions.FindAction(TrickButtonCName),

            PauseButton = InputSystem.actions.FindAction(PauseButtonName),

            DriftButton = InputSystem.actions.FindAction(DriftButtonName),
            BoostRamButton = InputSystem.actions.FindAction(BoostRamButtonName)
        };
    }
}
