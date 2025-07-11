using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class JumpPad : MonoBehaviour
    {
        [Header("Jump Pad Settings")]
        public float forwardForce = 20f;
        public float upwardForce = 10f;
        public float activationTime = 0.15f;

        private Dictionary<Rigidbody, float> stayTimers = new();

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Rigidbody rb = other.attachedRigidbody;
            if (rb == null)
                return;

            if (!stayTimers.ContainsKey(rb))
            {
                stayTimers[rb] = 0f;
            }

            stayTimers[rb] += Time.deltaTime;

            if (stayTimers[rb] >= activationTime)
            {
                Vector3 launchVelocity = transform.forward * forwardForce + Vector3.up * upwardForce;
                rb.linearVelocity = launchVelocity;

                rb.transform.rotation = Quaternion.LookRotation(transform.forward);

                stayTimers.Remove(rb);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && stayTimers.ContainsKey(rb))
            {
                stayTimers.Remove(rb);
            }
        }
    }
}
