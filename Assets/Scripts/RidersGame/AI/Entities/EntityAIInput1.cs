namespace RidersRuntime.Input
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using RidersRuntime.VehicleSystem;
    using System.Data;

    public class EntityAIInput1 : MonoBehaviour
    {
        [Header("Path Settings")]
        [SerializeField] private NodePath path;
        [SerializeField] private float nodeReachThreshold = 2f;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float turnSpeed = 5f;
        [SerializeField] private float gravityMultiplier = 25f;

        private Rigidbody rb;
        private int currentNodeIndex = 0;
        private Vector3 currentTarget;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();

            if (path == null || path.nodes == null || path.nodes.Length == 0)
            {
                Debug.LogError("[EntityAIInput] NodePath not assigned or empty.");
                enabled = false;
                return;
            }

            currentTarget = path.nodes[currentNodeIndex].position;
        }

        void Update()
        {
            Vector3 toTarget = currentTarget - transform.position;
            Vector3 horizontalToTarget = new Vector3(toTarget.x, 0f, toTarget.z);

            // Advance to next node if close enough
            if (horizontalToTarget.sqrMagnitude < nodeReachThreshold * nodeReachThreshold)
            {
                AdvanceToNextNode();
                return;
            }

            Vector3 moveDir = horizontalToTarget.normalized;

            // Rotate to face movement direction
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }

            // Apply movement (keep vertical velocity intact)
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 desiredVelocity = moveDir * moveSpeed;
            desiredVelocity.y = currentVelocity.y;

            // NaN check (safety)
            if (float.IsNaN(desiredVelocity.x) || float.IsNaN(desiredVelocity.y) || float.IsNaN(desiredVelocity.z))
            {
                Debug.LogWarning("[EntityAIInput] Skipping frame due to NaN velocity.");
                return;
            }

            rb.linearVelocity = desiredVelocity;
        }

        void FixedUpdate()
        {
            // Custom gravity
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }

        private void AdvanceToNextNode()
        {
            currentNodeIndex = (currentNodeIndex + 1) % path.nodes.Length;
            currentTarget = path.nodes[currentNodeIndex].position;
        }
    }
}