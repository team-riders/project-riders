using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RidersRuntime.VehicleSystem;
using System.Data;
using System.IO.Compression;

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
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(EntityRigidbody.position, EntityRigidbody.transform.up * 50f);
        }
        ActorInputData inputData = new();
        void Update()
        {
            if (path == null || path.nodes.Length == 0) return;

            targetNode = path.nodes[currentNodeIndex];
            SetTargetPosition(targetNode.position);
            if (Vector3.Distance(EntityRigidbody.transform.position, targetNode.position) < nodeReachThreshold)
            {
                if (flipflag) { flip = !flip; }
                currentNodeIndex = (currentNodeIndex + 1) % path.nodes.Length;
                // if (currentNodeIndex == path.nodes.Length - 1) { FlipFlag(); }
            }
            Vector3 forward = EntityRigidbody.transform.forward;
            Vector3 up = EntityRigidbody.transform.up;
            Vector3 diffToTarget = targetPosition - EntityRigidbody.transform.position;
            if (flip) { forward = EntityRigidbody.transform.forward; }
            else { forward = -EntityRigidbody.transform.forward; }
            Debug.DrawLine(EntityRigidbody.transform.position, EntityRigidbody.transform.position + up * 10f, Color.red);
            distanceToTarget = diffToTarget.magnitude;
            float angleToTarget = Vector3.SignedAngle(diffToTarget, up, Vector3.up);
            Debug.Log("Angle to target: " + angleToTarget);
            //Debug.Log((Mathf.Clamp(angleToTarget / 45f, -1f, 1f) * Vector3.right));
            float turnInput = (angleToTarget /45f);
            Vector3 turnVector = up * turnInput;
            float accelerate = distanceToTarget > nodeAccelerateThreshold ? 1f : 0f;
            float speed = flip ? -10f : 10f;
            //EntityRigidbody.AddTorque(turnVector * 15f, ForceMode.Acceleration);
            //EntityRigidbody.linearVelocity = up * accelerate * speed;
            Debug.Log("TurnVector: " + turnVector * 5f);
            //EntityRigidbody.linearVelocity = new(accelerate * speed, 0, 0);
            // EntityRigidbody.AddTorque(turnVector * 3f, ForceMode.Acceleration);
            // Vector3 baseDirection = flip ? EntityRigidbody.transform.right : -EntityRigidbody.transform.right;
            // if(baseDirection.sqrMagnitude <0.001f) baseDirection = Vector3.right;
            // Vector3 velocity = EntityRigidbody.linearVelocity;
            // velocity.x = baseDirection.x * accelerate * Mathf.Abs(speed); 
            // EntityRigidbody.linearVelocity = velocity;
        }

        
        void FixedUpdate()
        {
            EntityRigidbody.AddForce(Physics.gravity * 25f, ForceMode.Acceleration);   
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