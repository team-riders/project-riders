using UnityEngine;

namespace RidersCore
{
    public class ApplicationSettings : MonoBehaviour
    {

        void Start()
        {
            QualitySettings.vSyncCount = 0; // Disable VSync
            Application.targetFrameRate = 60;
        }

    }
}