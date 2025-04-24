using UnityEngine;

public class MovementState
{
    protected BaseVehicle vehicle;
    protected MovementStateMachine movementStateMachine;

    public MovementState (BaseVehicle vehicle, MovementStateMachine movementStateMachine)
    {
        this.vehicle = vehicle;
        this.movementStateMachine = movementStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
}
