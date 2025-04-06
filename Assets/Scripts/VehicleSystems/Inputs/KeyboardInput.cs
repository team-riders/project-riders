using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInput : BaseInput
{
    public string TurnInputName = "Horizontal";
    public string AccelerateButtonName = "Accelerate";
    public string BrakeButtonName = "Brake";
    public string JumpButtonName = "Jump";
    public PlayerInput pInput;

    public void Start()
    {
        pInput = GetComponent<PlayerInput>();
    }

    public override InputData GenerateInput()
    {
        return new InputData
        {
            Accelerate = InputSystem.actions.FindAction(AccelerateButtonName),
            Brake = InputSystem.actions.FindAction(BrakeButtonName),
            TurnInput = InputSystem.actions.FindAction(TurnInputName),
            Jump = InputSystem.actions.FindAction(JumpButtonName),
        };
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        bool wantsToJump = ctx.ReadValue<bool>();
    }
}
