using System.Collections.Generic;
using UnityEngine;
using RidersRuntime.Input;
using RidersRuntime.Data;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BaseInput))]
    public partial class RideMovementController : MonoBehaviour
    {
        public VehicleStatsSO m_VehicleStats;
        public bool canMove = false;

        Rigidbody m_rigidbody;
        MovementStateMachineAlt m_stateMachine;
        ActorInputData inputIntention;
        IInput m_ActorInputComponent;
        PowerupController powerupController;

        void DefineStates()
        {
            // Ground Movement State
            GroundMovementState groundMovementState = new();
            AirborneMovementState airborneMovementState = new();
            GrindingMovementState grindingMovementState = new();

            List<MovementStateAlt> states = new() { groundMovementState, airborneMovementState, grindingMovementState };

            m_stateMachine = new MovementStateMachineAlt(states, () => { }, () => SendBlackboard());

            m_stateMachine.AddTransition(groundMovementState, airborneMovementState, () => GroundPercent <= 0f);
            m_stateMachine.AddTransition(airborneMovementState, groundMovementState, () => GroundPercent > 0f);
            m_stateMachine.AddTransition(airborneMovementState, grindingMovementState,
                () => m_GrindPathTarget != null && m_rigidbody.linearVelocity.y < grindingMovementState.minimumDownwardSpeed
            );

            m_stateMachine.AddTransition(grindingMovementState, airborneMovementState,
                () => !grindingMovementState.IsGrinding
            );

            foreach (MovementStateAlt state in states) state.Setup(gameObject);

            m_stateMachine.ForceTransition(groundMovementState);
        }

        void Start()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_ActorInputComponent = GetComponent<IInput>();

            powerupController = new(m_VehicleStats._vehicleStats);

            DefineStates();
        }

        void Update()
        {
            if (!canMove) return;
            ProcessAndBufferIntent();
            // Some states will depend on this
            PreCalcChecks();

            m_stateMachine.Update();
        }

        void FixedUpdate()
        {
            if (!canMove) return;
            PreCalcChecks();
            m_stateMachine.PhysicsUpdate();
        }

        void PreCalcChecks()
        {
            GetGroundedPercent();
            ExecuteExternalPrechecks?.Invoke();
            // Update the board

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
                Jump = (inputIntention.Jump || new_inputs.Jump) && GroundPercent >= 0.5f,
                JumpHoldDuration = new_inputs.JumpHoldDuration,
                StuntA = new_inputs.StuntA,
                StuntB = new_inputs.StuntB,
                StuntC = new_inputs.StuntC,

                Drift = new_inputs.Drift,
                BoostRam = new_inputs.BoostRam
            };
        }

        public void TurnOn() => canMove = true;
        public void TurnOff() => canMove = false;
        public bool IsOn() => canMove;
    }

}