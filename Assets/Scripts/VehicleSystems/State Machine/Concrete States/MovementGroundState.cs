using UnityEngine;

public class MovementGroundState : MovementState
{
    public MovementGroundState(BaseVehicle vehicle, MovementStateMachine movementStateMachine) : base(vehicle, movementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
