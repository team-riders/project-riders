using UnityEngine;
using TMPro;

namespace RidersCore.Unity.UI
{
    public class BaseVehicleDebug : MonoBehaviour
    {
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI angleText;

        Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
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