using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
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

            if (vehicle.m_CanMove)
            {
                vehicle.MoveVehicle(vehicle.Input.Accelerate == 1, vehicle.Input.Brake == 1, vehicle.Input.TurnInput, vehicle.WantsToJump, vehicle.WantsToJumpHold);
            }

            if (vehicle.m_InAir)
            {
                vehicle.StateMachine.ChangeState(vehicle.AirState);
            }
        }
    }
}