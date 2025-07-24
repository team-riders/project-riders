using UnityEngine;

namespace RidersRuntime
{
    public class TrailTest : MonoBehaviour
    {
        public TrailController controller;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            controller = GetComponent<TrailController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.T))
            {
                controller.EnableTrail();
            }
        }
    }
}
