using RidersRuntime.DebugTools;
using RidersRuntime.Input;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RidersRuntime.Analytics
{
    public class InputProcessor : MonoBehaviour
    {
        public BaseInput inputSource;
        public List<InputSnapshot> InputHistory { get; private set; } = new();
        private InputActionAsset playerInput;
        private InputActionMap inputMap;

        private ActorInputData? lastInput = null;
        private int lastInputTimeMs = -1;

        void Start()
        {
            playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>().actions;
            inputMap = playerInput.FindActionMap(DebugFlags.Instance.DebugInputMapName);
        }

        void Update()
        {
            ActorInputData currentInput = inputSource.GrabCurrentFrameInputs();
            int currentTimeMs = Mathf.FloorToInt(Time.time * 1000);

            if (lastInput == null || !currentInput.Equals(lastInput.Value))
            {
                // Close the previous input span
                if (lastInput != null && lastInputTimeMs != currentTimeMs)
                {
                    InputHistory.Add(new InputSnapshot(currentTimeMs - 1, lastInput.Value));
                }

                // Start a new input span
                InputHistory.Add(new InputSnapshot(currentTimeMs, currentInput));
                lastInput = currentInput;
                lastInputTimeMs = currentTimeMs;
            }

            // DEBUG - EXPORT TO CSV AND JSON
            if (DebugFlags.Instance.allowSavingInputHistoryToFile == true)
            {
                if (inputMap.FindAction("Save Input History").triggered)
                {
                    SaveHistoryAsCsv();
                    SaveHistoryAsJson();
                }
            }
        }

        private string LogsPath
        {
            get
            {
                string logsDir = Path.Combine(Application.dataPath, Paths.Logs);
                if (!Directory.Exists(logsDir)) Directory.CreateDirectory(logsDir);
                return logsDir;
            }
        }

        public void ClearHistory()
        {
            InputHistory.Clear();
        }

        ActorInputData GetInputAtTime(int timeMs)
        {
            for (int i = InputHistory.Count - 1; i >= 0; i--)
            {
                if (InputHistory[i].TimeMs <= timeMs)
                    return InputHistory[i].Input;
            }
            return default;
        }

        // DEBUG
        public void SaveHistoryAsCsv()
        {
            string path = Path.Combine(LogsPath, FileNames.InputCsv);
            StringBuilder csv = new StringBuilder();

            csv.AppendLine($"Time (ms)," +
                           $"{ButtonNamesShort.Accelerate}," +
                           $"{ButtonNamesShort.Brake}," +
                           $"{ButtonNamesShort.TurnInput}," +
                           $"{ButtonNamesShort.Jump}," +
                           $"{InputNameSpecial.JumpHoldDuration}," +
                           $"{StuntButtonNamesShort.StuntA}," +
                           $"{StuntButtonNamesShort.StuntB}," +
                           $"{StuntButtonNamesShort.StuntC}," +
                           $"{ButtonNamesShort.Drift}," +
                           $"{ButtonNamesShort.BoostRam}");

            foreach (var record in InputHistory)
            {
                var input = record.Input;
                csv.AppendLine($"{record.TimeMs}," +
                               $"{input.Accelerate}," +
                               $"{input.Brake}," +
                               $"{input.TurnInput}," +
                               $"{input.Jump}," +
                               $"{input.JumpHoldDuration}," +
                               $"{input.StuntA}," +
                               $"{input.StuntB}," +
                               $"{input.StuntC}," +
                               $"{input.Drift}," +
                               $"{input.BoostRam}");
            }

            File.WriteAllText(path, csv.ToString());
            Debug.Log($"Input history saved to {path}");
        }

        // DEBUG
        public void SaveHistoryAsJson()
        {
            string path = Path.Combine(LogsPath, FileNames.InputJson);

            InputHistoryData historyData = new InputHistoryData
            {
                inputHistory = InputHistory
            };

            string json = JsonUtility.ToJson(historyData, true);

            File.WriteAllText(path, json);
            Debug.Log($"Input history saved to {path}");
        }

    }
}