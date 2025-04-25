using UnityEngine;

public class MovementStateMachine
{
    public MovementState CurrentMovementState { get; set; }

    public void Initialise(MovementState startingState)
    {
        CurrentMovementState = startingState;
        CurrentMovementState.EnterState();
    }

    public void ChangeState(MovementState newState)
    {
        CurrentMovementState.ExitState();
        CurrentMovementState = newState;
        CurrentMovementState.EnterState();
    }
}
