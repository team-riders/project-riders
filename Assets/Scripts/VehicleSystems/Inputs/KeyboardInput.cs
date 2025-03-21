using UnityEngine;

public class KeyboardInput : BaseInput
{
    public string TurnInputName = "Horizontal";
    public string AccelerateButtonName = "Accelerate";
    public string BrakeButtonName = "Brake";
    public string JumpButtonName = "Jump";

    public override InputData GenerateInput()
    {
        return new InputData
        {
            Accelerate = Input.GetButton(AccelerateButtonName),
            Brake = Input.GetButton(BrakeButtonName),
            TurnInput = Input.GetAxis("Horizontal"),
            Jump = Input.GetButton(JumpButtonName),
        };
    }
}
