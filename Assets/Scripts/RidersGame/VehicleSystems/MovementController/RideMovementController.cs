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
        IInput m_ActorInputComponent;
        PowerupController powerupController;

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
            if (!canMove) return;
            PreCalcChecks();
            m_stateMachine.Update();
            // Logic update happens here with the state machine
            // In reality nothing actually happens here for now, we are just doing the transitions.

            MovementStateAlt state = m_stateMachine.CurrentState;
            Debug.Log($"Current State: {state.Name}");
            // Pass any information we need to the states here.
            state.ComputePhysicsIntentions(SendBlackboard());
        }

        void PreCalcChecks()
        {
            GetGroundedPercent();
            // Update any information we need to provide to the states
            ExecuteExternalPrechecks?.Invoke();
        }

        void ProcessAndBufferIntent()
        {
            ActorInputData new_inputs = m_ActorInputComponent.GrabCurrentFrameInputs();

            inputIntention = new()
            {
                Accelerate = new_inputs.Accelerate,
                Brake = new_inputs.Brake,
                TurnInput = new_inputs.TurnInput,
                // TODO: MOVE THIS TO THE INPUT PROCESSOR INSTEAD
                // Force jump to be a "consumable" intention.
                // i.e. Jump will only turn false if 
                //     1. We are not trying to jump
                //     2. The "jump" we set up has been consumed (changed to false)
                // ! JUMP IS BROKEN, NEEDS TO BE FIXED
                Jump = inputIntention.Jump || new_inputs.Jump,
                JumpHoldDuration = new_inputs.JumpHoldDuration,
                StuntA = new_inputs.StuntA,
                StuntB = new_inputs.StuntB,
                StuntC = new_inputs.StuntC,

                Drift = new_inputs.Drift,
                BoostRam = new_inputs.BoostRam
            };
        }

        public void AddExternalPrecheck(Action action)
        {
            if (action == null) return;
            ExecuteExternalPrechecks += action;
        }
    }
}