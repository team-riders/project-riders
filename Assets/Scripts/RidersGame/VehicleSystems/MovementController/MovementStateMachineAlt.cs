using System;
using System.Collections.Generic;
using UnityEngine;
using RidersRuntime.Input;
using RidersRuntime.VehicleSystem;

public class MovementStateMachineAlt : StateMachine<MovementStateAlt>
{
    private Action<Vector3, Vector3> DoExecute;
    public MovementStateMachineAlt(List<MovementStateAlt> states, Action<Vector3, Vector3> DoMovementAndRotation) : base(states)
    {
        DoExecute = DoMovementAndRotation;
    }
    ActorInputData inputIntention;

    public void PhysicsUpdate()
    {
        if (CurrentState == null) return;

        CurrentState.ComputeIntention(inputIntention, out Vector3 intent_velocity, out Vector3 intent_rotation);

        DoExecute(intent_velocity, intent_rotation);
    }

    public void CacheInputIntention(ActorInputData intent)
    {
        inputIntention = intent;
    }
}