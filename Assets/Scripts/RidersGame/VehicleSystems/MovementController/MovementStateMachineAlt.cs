using System;
using System.Collections.Generic;
using RidersRuntime.VehicleSystem;
using UnityEngine;

public class MovementStateMachineAlt : StateMachine<MovementStateAlt>
{
    private Action DoExecute;

    private Func<Blackboard> _getExternalData;
    public MovementStateMachineAlt(List<MovementStateAlt> states, Action DoExecute, Func<Blackboard> getExternalData) : base(states)
    {
        this.DoExecute = DoExecute;
        _getExternalData = getExternalData;
    }

    public void PhysicsUpdate()
    {
        if (CurrentState == null) return;

        CurrentState.ComputeIntention(_getExternalData?.Invoke());
        Debug.Log($"Current State: {CurrentState.Name}");

        DoExecute?.Invoke();
    }
}