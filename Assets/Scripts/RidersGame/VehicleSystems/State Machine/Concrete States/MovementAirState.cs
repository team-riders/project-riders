using UnityEngine;

namespace RidersCore.VehicleSystem
{
    public class MovementAirState : MovementState
    {
        public MovementAirState(BaseVehicle vehicle, MovementStateMachine movementStateMachine) : base(vehicle, movementStateMachine)
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
            vehicle.GroundAirbourne();

            if (!vehicle.m_InAir)
            {
                vehicle.StateMachine.ChangeState(vehicle.GroundState);
            }
        }
    }
}