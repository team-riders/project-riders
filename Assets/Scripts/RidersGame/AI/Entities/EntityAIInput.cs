using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RidersRuntime.VehicleSystem;

namespace RidersRuntime.Input
{
public class EntityAIInput : MonoBehaviour
{
    InputActionAsset m_playerInput;
    [SerializeField] private EntityNodePath path;
    [SerializeField] private float nodeReachThreshold = 3f;
    [SerializeField] private float nodeCloseThreshold = 2f;
    [SerializeField] private float nodeAccelerateThreshold = 3.5f;
    [SerializeField] private Rigidbody EntityRigidbody;
    private Vector3 targetPosition;
   
    private int currentNodeIndex = 0;
    public float distanceToTarget;
    public Transform targetNode;
    private Node currentJumpNode;
    [SerializeField] private bool flip = true;
    [SerializeField] private bool flipflag = false;
    void Awake()
    {
        EntityRigidbody = GetComponent<Rigidbody>();
    }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(EntityRigidbody.position, EntityRigidbody.transform.forward * 50f);
        }
        ActorInputData inputData = new();
        void Update()
        {
            if (path == null || path.nodes.Length == 0) return;

            targetNode = path.nodes[currentNodeIndex];
            SetTargetPosition(targetNode.position);
            if (Vector3.Distance(this.transform.position, targetNode.position) < nodeReachThreshold)
            {
                if (flipflag) { flip = !flip; }
                currentNodeIndex = (currentNodeIndex + 1) % path.nodes.Length;
                if (currentNodeIndex == path.nodes.Length - 1) { FlipFlag(); }
            }

            Vector3 diffToTarget = targetPosition - this.transform.position;
            Vector3 forward = this.transform.forward;

            distanceToTarget = diffToTarget.magnitude;
            float angleToTarget = Vector3.SignedAngle(forward, diffToTarget, Vector3.up);

            float turnInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
            float accelerate = distanceToTarget > nodeAccelerateThreshold ? 1f : 0f;
            float brake = distanceToTarget < nodeCloseThreshold ? 1f : 0f;
            float speed = flip ? -15f : 15f;
            EntityRigidbody.linearVelocity = new(accelerate * speed, 0, 0);
    }

    private void FlipFlag()
    {
        flipflag = !flipflag;    
    }

    public void SetTargetPosition(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }
}
}