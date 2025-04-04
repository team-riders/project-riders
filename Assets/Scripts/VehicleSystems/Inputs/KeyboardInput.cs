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
            Accelerate = pInput.actions.FindAction(AccelerateButtonName),
            Brake = pInput.actions.FindAction(BrakeButtonName),
            TurnInput = pInput.actions.FindAction(TurnInputName),
            Jump = pInput.actions.FindAction(JumpButtonName),
        };
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        bool wantsToJump = ctx.ReadValue<bool>();
    }
}
