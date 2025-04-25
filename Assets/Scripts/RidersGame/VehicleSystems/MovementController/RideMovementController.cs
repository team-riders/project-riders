using System;
using System.Collections.Generic;
using UnityEngine;
using RidersRuntime.Input;
using RidersRuntime.Data;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BaseInput))]
    public class RideMovementController : MonoBehaviour
    {
        public List<MovementStateAlt> m_states = new()
        {
            new GroundMovementState()
            // Add other states here
        };
        public VehicleStatsSO m_VehicleStats;
        public bool isOn = false;

        Rigidbody m_rb;
        MovementStateMachineAlt m_stateMachine;
        ActorInputData inputIntention;
        IInput m_ActorInputComponent;

        void Start()
        {
            m_rb = GetComponent<Rigidbody>();
            m_ActorInputComponent = GetComponent<IInput>();

            foreach (MovementStateAlt state in m_states) state.Setup(m_VehicleStats._vehicleStats, m_rb);

            m_stateMachine = new MovementStateMachineAlt(m_states, DoMovementAndRotation);
        }

        void DoMovementAndRotation(Vector3 intent_velocity, Vector3 intent_rotation)
        {
            // We have this here if there are certain caps that we need to fix that apply globally
            m_rb.linearVelocity = intent_velocity;
            m_rb.angularVelocity = intent_rotation;
        }

        void Update()
        {
            if (!isOn) return;
            ProcessAndBufferIntent();
            m_stateMachine.CacheInputIntention(inputIntention);

            // Logic processing happens here
            // TODO: Figure out whether we have the capabilities to change state in the input step
            m_stateMachine.Update();
        }

        void FixedUpdate()
        {
            if (!isOn) return;
            m_stateMachine.PhysicsUpdate();
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
                Jump = inputIntention.Jump || new_inputs.Jump,
                JumpHoldDuration = new_inputs.JumpHoldDuration,
                StuntA = new_inputs.StuntA,
                StuntB = new_inputs.StuntB,
                StuntC = new_inputs.StuntC,

                Drift = new_inputs.Drift,
                BoostRam = new_inputs.BoostRam
            };
        }

        public Action ConsumeIntention(string key)
        {
            switch (key)
            {
                case "Jump":
                    inputIntention.Jump = false;
                    break;
                case "StuntA":
                    inputIntention.StuntA = false;
                    break;
                case "StuntB":
                    inputIntention.StuntB = false;
                    break;
                case "StuntC":
                    inputIntention.StuntC = false;
                    break;
                default:
                    Debug.LogWarning($"Unknown intention key: {key}");
                    break;
            }
            return null;
        }

        public void TurnOn() => isOn = true;
        public void TurnOff() => isOn = false;
        public bool IsOn() => isOn;

    }
}