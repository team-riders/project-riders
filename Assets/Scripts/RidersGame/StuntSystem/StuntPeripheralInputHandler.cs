using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RidersRuntime.Data;
using RidersRuntime.Input;
// Sole duty of this is to convert inputs into frame data to send to the stunt system
namespace RidersRuntime.StuntSystem
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(StuntSystem))]
    public class StuntPeripheralInputHandler : MonoBehaviour
    {
        StuntSystem stuntSystem;
        IInput playerPeripheralInput;
        public bool isInRecordingMode = false;
        List<FrameKeyData> recordedKeys = new();

        StuntType currentStunt = StuntType.None;
        int pollFrameHoldCounter = 0;

        void Start()
        {
            playerPeripheralInput = GetComponent<PlayerInput>();
            stuntSystem = GetComponent<StuntSystem>();
            Application.targetFrameRate = 60;
        }

        public void Initialize()
        {
            isInRecordingMode = true;
        }

        void Update()
        {
            if (isInRecordingMode)
            {
                ActorInputData currentFrameData = playerPeripheralInput.GrabCurrentFrameInputs();
                // Interpret this and record it down

                // If any has been pressed we can handle all cases 
                List<string> lastPressedKeys = RecordKeyFrames(currentFrameData, out bool isNew);
                List<string> stuntIntersect = lastPressedKeys.Intersect(StuntButtonNamesShort.AllStuntButtons).ToList();
                if (stuntIntersect.Count() > 0 && isNew)
                {
                    HandleStuntKeyPress(stuntIntersect[0]);
                }
            }
        }

        void HandleStuntKeyPress(string key)
        {
            currentStunt = Values.stuntBindings[key];
            // Build print out the actions for the last 10 actions
            List<FrameKeyData> keys = recordedKeys.Skip(Mathf.Max(0, recordedKeys.Count - 10)).ToList();

            stuntSystem.OnStuntRequestWithFrameData(keys, currentStunt);
            currentStunt = StuntType.None;
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

        List<string> RecordKeyFrames(ActorInputData input, out bool isNew)
        {
            isNew = false;
            // 1. Convert from 3 var input to 1 var 8 way input
            // Need to reimplement this - adapter to 8 way
            float horizontal = input.TurnInput;
            float vertical = input.Accelerate;
            vertical = vertical > 0 ? vertical : -1 * input.Brake;

            int direction = ConvertVector2To8WayDirection(new(horizontal, vertical));

            // 2. Get all positive inputs
            List<string> currentFrameKeys = input.KeysThatAreNonZeroExceptAnalog();
            currentFrameKeys.Add(direction.ToString());

            // Grab information about the previous frame
            List<string> previousFrameKeys = recordedKeys.Count > 0 ? recordedKeys.Last().inputs : new List<string>();
            // Check if the current frame keys are different from the previous frame keys
            if (currentFrameKeys.SequenceEqual(previousFrameKeys))
            {
                pollFrameHoldCounter++;
                // Replace the last entry in the list with the current frame keys
                recordedKeys[^1] = new FrameKeyData
                {
                    frameCount = pollFrameHoldCounter,
                    inputs = currentFrameKeys
                };
            }
            else
            {
                pollFrameHoldCounter = 1;
                isNew = true;
                recordedKeys.Add(new FrameKeyData
                {
                    frameCount = 1,
                    inputs = currentFrameKeys
                });
            }

            return currentFrameKeys;
        }


        // Function to get the last pressed keys from a list of lists of keystrokes
        public static List<string> GetLastPressedKeys(List<List<string>> keys)
        {
            List<string> lastPressedKeys = new();
            List<string> previous = new();

            foreach (var keystrokes in keys)
            {
                var newKeys = keystrokes.Except(previous).ToList();
                if (newKeys.Count > 0) lastPressedKeys.Add(newKeys[0]);

                previous = keystrokes;
            }

            return lastPressedKeys;
        }

        public static void ShowPressedKeys(List<FrameKeyData> keys)
        {
            foreach (var key in keys)
            {
                Debug.Log($"Frame: {key.frameCount}, Keys: {string.Join(", ", key.inputs)}");
            }
        }
        public static void ShowSingleLineOutput(List<FrameKeyData> keys)
        {
            List<List<string>> keyInputs = new();
            foreach (var frameData in keys)
            {
                keyInputs.Add(frameData.inputs);
            }
            List<string> lastPressedKeys = GetLastPressedKeys(keyInputs);
            Debug.Log($"Last pressed keys: {string.Join(", ", lastPressedKeys)}");
        }
    }
}