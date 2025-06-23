using RidersRuntime.Input;
using RidersRuntime.VehicleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// Sole duty of this is to convert inputs into frame data to send to the stunt system
namespace RidersRuntime.StuntSystem
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(StuntSystem))]
    [RequireComponent(typeof(RideMovementController))]
    public class StuntPeripheralInputHandler : MonoBehaviour
    {
        RideMovementController rideMovementController;
        StuntSystem stuntSystem;
        IInput playerPeripheralInput;
        public bool isInRecordingMode = false;
        private Queue<TimedInput> inputBuffer = new();
        private List<string> previousFrameKeys = new();

        StuntType currentStunt = StuntType.None;

        void Start()
        {
            playerPeripheralInput = GetComponent<PlayerInput>();
            stuntSystem = GetComponent<StuntSystem>();
            rideMovementController = GetComponent<RideMovementController>();
        }

        public void Initialize()
        {
            isInRecordingMode = true;
        }

        void Update()
        {
            if (!isInRecordingMode) return;

            var state = rideMovementController.GetCurrentMovementState();
            switch (state)
            {
                case GroundMovementState:
                    break;
                case AirborneMovementState:
                    RecordInputs();
                    break;
                case GrindingMovementState:
                    RecordInputs();
                    break;
            }
        }

        void RecordInputs()
        {
            // Prune old inputs
            while (inputBuffer.Count > 0 && Time.time - inputBuffer.Peek().time > Values.timeWindowMax)
                inputBuffer.Dequeue();

            // Get new inputs
            var current = playerPeripheralInput.GrabCurrentFrameInputs();
            var keys = GetCurrentKeys(current);
            foreach (var key in keys)
                inputBuffer.Enqueue(new TimedInput { key = key, time = Time.time });

            List<string> newKeys = keys.Except(previousFrameKeys).ToList();
            var newStuntKey = newKeys.FirstOrDefault(k => StuntButtonNamesShort.AllStuntButtons.Contains(k));
            if (newStuntKey != null)
            {
                HandleStuntKeyPress(newStuntKey);
            }
            previousFrameKeys = keys;
        }

        void HandleStuntKeyPress(string key)
        {
            currentStunt = Values.stuntBindings[key];
            stuntSystem.OnStuntRequestTimed(new List<TimedInput>(inputBuffer), currentStunt);
            currentStunt = StuntType.None;
        }

        List<string> GetCurrentKeys(ActorInputData input)
        {
            float horizontal = input.TurnInput;
            float vertical = input.Accelerate > 0 ? input.Accelerate : -1 * input.Brake;
            int dir = ConvertVector2To8WayDirection(new Vector2(horizontal, vertical));
            var keys = input.KeysThatAreNonZeroExceptAnalog();
            keys.Add(dir.ToString());
            return keys;
        }

        int ConvertVector2To8WayDirection(Vector2 input)
        {
            float deadzone = 0.5f;

            if (input.x < deadzone && input.x > -deadzone && input.y < deadzone && input.y > -deadzone)
            {
                return 5; // Neutral
            }
            if (input.x > deadzone && input.y > deadzone) return 9; // Up-Right
            if (input.x > deadzone && input.y < -deadzone) return 3; // Down-Right
            if (input.x < -deadzone && input.y > deadzone) return 7; // Up-Left
            if (input.x < -deadzone && input.y < -deadzone) return 1; // Down-Left
            if (input.x > deadzone) return 6; // Right
            if (input.x < -deadzone) return 4; // Left
            if (input.y > deadzone) return 8; // Up
            if (input.y < deadzone) return 2; // Down

            return -1; // No valid direction
        }
    }
}