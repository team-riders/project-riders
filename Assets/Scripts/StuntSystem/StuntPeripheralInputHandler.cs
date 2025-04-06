using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// Sole duty of this is to convert inputs into frame data to send to the stunt system

public class StuntPeripheralInputHandler : MonoBehaviour
{
    StuntSystem stuntSystem;
    InputActionAsset inputActions;
    bool isInRecordingMode = false;

    string[] stuntKeyBindings = new string[] { "Player/Trick Button A", "Player/Trick Button B", "Player/Trick Button C" };
    string[] directionalKeyBindings = new string[] { "Accelerate", "Brake", "Horizontal" };
    Dictionary<string, StuntType> stuntBindings = new()
    {
        { "Trick Button A", StuntType.Flip },
        { "Trick Button B", StuntType.Grind },
        { "Trick Button C", StuntType.Spin }
    };
    List<FrameKeyData> recordedKeys = new();

    StuntType currentStunt = StuntType.None;
    int pollFrameHoldCounter = 0;

    void Start()
    {
        Application.targetFrameRate = 60;
        inputActions = GetComponent<PlayerInput>().actions;
        stuntSystem = GetComponent<StuntSystem>();

        Initialize();
    }

    public void Initialize()
    {
        isInRecordingMode = true;
    }

    void Update()
    {
        if (isInRecordingMode)
        {
            PollEveryKey();

            // If any has been pressed we can handle all cases 
            foreach (var key in stuntKeyBindings)
            {
                if (inputActions.FindAction(key).triggered && inputActions.FindAction(key).IsPressed() && currentStunt == StuntType.None)
                {
                    HandleStuntKeyPress(key);
                    return;
                }
            }
        }
    }

    void HandleStuntKeyPress(string key)
    {
        string keyName = key.Split('/')[1];

        currentStunt = stuntBindings[keyName];
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

    // Implemented for analytics anyways, but we want this implementation to be independent?
    void PollEveryKey()
    {
        List<string> currentFrameKeys = new();
        foreach (var action in inputActions.FindActionMap("Player").actions)
        {
            if (action.triggered && !directionalKeyBindings.Contains(action.name))
            {
                currentFrameKeys.Add(action.name);
            }
        }

        // Need to reimplement this
        float horizontal = inputActions.FindAction("Player/Horizontal").ReadValue<float>();
        float vertical = inputActions.FindAction("Player/Accelerate").ReadValue<float>();
        vertical = vertical > 0 ? vertical : -1 * inputActions.FindAction("Player/Brake").ReadValue<float>();
        Vector2 input = new(horizontal, vertical);

        int direction = ConvertVector2To8WayDirection(input);
        if (direction != -1)
        {
            currentFrameKeys.Add(direction.ToString());
        }

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
            recordedKeys.Add(new FrameKeyData
            {
                frameCount = 1,
                inputs = currentFrameKeys
            });
        }
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
        List<string> lastPressedKeys = StuntPeripheralInputHandler.GetLastPressedKeys(keyInputs);
        Debug.Log($"Last pressed keys: {string.Join(", ", lastPressedKeys)}");
    }
}