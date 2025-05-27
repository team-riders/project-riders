using RidersRuntime.Data;
using RidersRuntime.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BaseInput))]
    public partial class RideMovementController : MonoBehaviour
    {
        public VehicleStatsSO m_VehicleStats;
        public bool canMove = true;
        Rigidbody m_rigidbody;
        StateMachine<MovementStateAlt> m_stateMachine;
        ActorInputData inputIntention;
        bool JumpIntention = false;
        IInput m_ActorInputComponent;
        public PowerupController powerupController;

        // Eventually serialize this to show internal function properties
        GrindingMovementState grindingMovementState = new();
        GroundMovementState groundMovementState = new();
        AirborneMovementState airborneMovementState = new();

        public void TurnOn() => canMove = true;
        public void TurnOff() => canMove = false;
        public bool IsOn() => canMove;

        void DefineStates()
        {
            List<MovementStateAlt> states = new() { groundMovementState, airborneMovementState, grindingMovementState };

            m_stateMachine = new(states);

            m_stateMachine.AddTransition(groundMovementState, airborneMovementState, () => GroundPercent <= 0f);
            m_stateMachine.AddTransition(airborneMovementState, groundMovementState, () => GroundPercent > 0f);
            m_stateMachine.AddTransition(airborneMovementState, grindingMovementState,
                TransitionCanGrind
            );

            m_stateMachine.AddTransition(grindingMovementState, airborneMovementState,
                () => !grindingMovementState.IsGrinding
            );

            foreach (MovementStateAlt state in states) state.Setup(gameObject);

            AddExternalPrecheck(grindingMovementState.DoCooldown);

            m_stateMachine.ForceTransition(groundMovementState);
        }

        bool TransitionCanGrind()
        {
            return m_GrindPathTarget != null
                && m_rigidbody.linearVelocity.y < grindingMovementState.minimumDownwardSpeed
                && grindingMovementState.CanEnterGrind();
        }

        void Start()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_ActorInputComponent = GetComponent<IInput>();

            powerupController = new(m_VehicleStats._vehicleStats);
            Debug.Log("powerupcontroller: " + powerupController);
            SetCenterOfMass();

            DefineStates();
        }

        void Update()
        {
            if (!canMove) return;
            ProcessAndBufferIntent();
            // Some states will depend on this
            PreCalcChecks();
        }

        void FixedUpdate()
        {
            powerupController.TickPowerups();
            if (!canMove) return;
            PreCalcChecks();
            m_stateMachine.Update();

            MovementStateAlt state = m_stateMachine.CurrentState;
            state.ComputePhysicsIntentions(SendBlackboard());
            Blackboard data = state.ReturnStateBlackboard();
            PostExecuteBlackboardChange(data);
        }

        void PreCalcChecks()
        {
            GetGroundedPercent();
            ExecuteExternalPrechecks?.Invoke();
        }

        void ProcessAndBufferIntent()
        {
            inputIntention = m_ActorInputComponent.GrabCurrentFrameInputs();
            JumpIntention = JumpIntention || inputIntention.Jump;
        }

        public void AddExternalPrecheck(Action action)
        {
            if (action == null) return;
            ExecuteExternalPrechecks += action;
        }

        public void PostExecuteBlackboardChange(Blackboard blackboard)
        {
            if (blackboard.GetValue<bool>("JumpIntention") == false)
            {
                JumpIntention = false;
            }
        }

        public MovementStateAlt GetCurrentMovementState()
        {
            return m_stateMachine.CurrentState;
        }

        // Added: Method to expose current speed in km/h for RacerComponent
        public float GetCurrentSpeed()
        {
            return m_rigidbody != null ? m_rigidbody.linearVelocity.magnitude * 3.6f : 0f;
        }
    }
}
