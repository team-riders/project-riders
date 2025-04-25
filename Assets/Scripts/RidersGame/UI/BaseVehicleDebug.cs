using UnityEngine;
using TMPro;

namespace RidersRuntime.UI
{
    // Need to rework soon to decouple AWAY from the vehicle
    public class BaseVehicleDebug : MonoBehaviour
    {
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI angleText;

        Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            // FindTarget()
        }

        void FindTarget()
        {
            // Find the target vehicle in the scene
            GameObject targetVehicle = GameObject.FindGameObjectWithTag("PlayerVehicle");
            if (targetVehicle != null)
            {
                rb = targetVehicle.GetComponent<Rigidbody>();
            }
            else
            {
                Debug.LogWarning("Target vehicle not found in the scene.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (rb != null)
            {
                speedText.text = "Speed: " + rb.linearVelocity.magnitude.ToString("F2") + " m/s";
                angleText.text = "Angle: " + transform.eulerAngles.y.ToString("F2") + "°";
            }
            else
            {
                speedText.text = "Speed: N/A";
                angleText.text = "Angle: N/A";
            }
        }
    }
}