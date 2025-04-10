using System.IO;
using System.Reflection;
using UnityEngine;

public class DebugFlags : MonoBehaviour
{
    public static DebugFlags Instance { get; private set; }

    // This is needed to be able to save/load from a JSON config file
    [System.Serializable]
    private class DebugFlagData
    {
        public bool allowSavingInputHistoryToFile = false;

        // Add any more debug toggles here. AND UNDER THE HEADER BELOW.
        // Then in other scripts, you can access the state with DebugFlags.Instance.Name
    }

    [Header("Debug Toggles")]
    public bool allowSavingInputHistoryToFile = false;

    private string ConfigPath
    {
        get
        {
            string configDir = Path.Combine(Application.dataPath, "Config");
            if (!Directory.Exists(configDir)) Directory.CreateDirectory(configDir);
            return Path.Combine(configDir, "debug_flags.json");
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadFromFile();
    }

    private void OnDisable()
    {
        SaveToFile();
    }

    private void Start()
    {
        Debug.Log("[DebugFlags] Current Debug Toggles:");
        PrintCurrentToggleValues();
    }

    private void PrintCurrentToggleValues()
    {
        // Print current values of the toggles
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            Debug.Log($"{field.Name}: {field.GetValue(this)}");
        }
    }

    public void SaveToFile()
    {
        var data = new DebugFlagData();

        foreach (var field in typeof(DebugFlagData).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            var thisField = GetType().GetField(field.Name, BindingFlags.Instance | BindingFlags.Public);
            if (thisField != null)
            {
                field.SetValue(data, thisField.GetValue(this));
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(ConfigPath, json);
        Debug.Log($"[DebugFlags] Saved to: {ConfigPath}");
    }

    public void LoadFromFile()
    {
        if (!File.Exists(ConfigPath))
        {
            Debug.Log("[DebugFlags] No config found. Using default toggle values.");
            return;
        }

        string json = File.ReadAllText(ConfigPath);
        var data = JsonUtility.FromJson<DebugFlagData>(json);

        foreach (var field in typeof(DebugFlagData).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            var thisField = GetType().GetField(field.Name, BindingFlags.Instance | BindingFlags.Public);
            if (thisField != null)
            {
                thisField.SetValue(this, field.GetValue(data));
            }
        }

        Debug.Log($"[DebugFlags] Loaded toggles from: {ConfigPath}");
    }
}
