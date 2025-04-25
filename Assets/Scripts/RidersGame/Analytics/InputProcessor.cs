using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using RidersCore.Data;
using RidersCore.Input;
using RidersCore.DebugTools;

namespace RidersCore.Analytics
{
    // For JSON serialisation
    [System.Serializable]
    public class InputHistoryData
    {
        public List<InputFrameRecord> inputHistory = new();
    }

    public class InputProcessor : MonoBehaviour
    {
        public BaseInput inputSource;
        public List<InputFrameRecord> InputHistory { get; private set; } = new();
        private InputActionAsset playerInput;
        private InputActionMap inputMap;

        void Awake()
        {
            playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>().actions;
            inputMap = playerInput.FindActionMap(DebugFlags.Instance.DebugInputMapName);
        }

        void Update()
        {
            ActorInputData currentInput = inputSource.GrabCurrentFrameInputs();
            int currentFrame = Time.frameCount;

            var record = new InputFrameRecord(currentFrame, currentInput);

            InputHistory.Add(record);

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
                string logsDir = Path.Combine(Application.dataPath, "Logs");
                if (!Directory.Exists(logsDir)) Directory.CreateDirectory(logsDir);
                return logsDir;
            }
        }

        public void ClearHistory()
        {
            InputHistory.Clear();
        }

        public InputFrameRecord GetFrame(int frame)
        {
            return InputHistory.Find(record => record.Frame == frame);
        }

        // DEBUG
        public void SaveHistoryAsCsv()
        {
            string path = Path.Combine(LogsPath, "InputHistory.csv");
            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Frame,Accelerate,Brake,TurnInput,Jump,JumpHoldDuration,StuntA,StuntB,StuntC,Drift,BoostRam");

            foreach (var record in InputHistory)
            {
                var input = record.Input;
                csv.AppendLine($"{record.Frame}," +
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
            string path = Path.Combine(LogsPath, "InputHistory.json");

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