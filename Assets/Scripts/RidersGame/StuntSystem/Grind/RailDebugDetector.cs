using UnityEngine;

namespace RidersRuntime.DebugTools
{

    [DisallowMultipleComponent]
    public class RailDebugDetector : MonoBehaviour
    {
        [Header("Debug Settings")]
        public bool logEnter = true;
        public bool logStay = false;
        public bool logExit = true;
        public string tagFilter = ""; // Leave empty for all

        private void OnTriggerEnter(Collider other)
        {
            if (logEnter && IsValid(other))
            {
                Debug.Log($"[Rail DEBUG] 🚀 '{gameObject.name}' was triggered by: {other.name}");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (logStay && IsValid(other))
            {
                Debug.Log($"[Rail DEBUG] 👣 '{gameObject.name}' is being touched by: {other.name}");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (logExit && IsValid(other))
            {
                Debug.Log($"[Rail DEBUG] 👋 '{gameObject.name}' no longer touched by: {other.name}");
            }
        }

        private bool IsValid(Collider other)
        {
            return string.IsNullOrEmpty(tagFilter) || other.CompareTag(tagFilter);
        }
    }
}