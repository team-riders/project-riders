using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RidersRuntime.VehicleSystem;

namespace RidersRuntime.Input
{
public class AIInput : BaseInput
{
    [SerializeField] private NodePath path;
    [SerializeField] private float nodeReachThreshold = 3f;
    [SerializeField] private float nodeCloseThreshold = 5f;
    private BaseVehicle baseVehicle;
    private Vector3 targetPosition;
   
    private int currentNodeIndex = 0;
    public float distanceToTarget;
    public Transform targetNode;
    void Awake()
    {
        baseVehicle = GetComponent<BaseVehicle>();
    }
    
    ActorInputData inputData = new();
    void Update()
    {
        if (path == null || path.nodes.Length == 0) return;

        targetNode = path.nodes[currentNodeIndex];
        SetTargetPosition(targetNode.position);
        if (Vector3.Distance(baseVehicle.transform.position, targetNode.position) < nodeReachThreshold)
        {
            currentNodeIndex = (currentNodeIndex + 1) % path.nodes.Length; // loop path
            
        }
        inputData = FollowNodes();
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    ActorInputData FollowNodes()
    {
        Vector3 diffToTarget = targetPosition - baseVehicle.transform.position;
        Vector3 forward = baseVehicle.transform.forward;

        distanceToTarget = diffToTarget.magnitude;
        float angleToTarget = Vector3.SignedAngle(forward, diffToTarget, Vector3.up);

        float turnInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        float accelerate = distanceToTarget > nodeCloseThreshold ? 1f : 0f;
        float brake = distanceToTarget < nodeReachThreshold ? 1f : 0f;
        
        return new ActorInputData
        {
            Accelerate = accelerate,
            Brake = brake,
            TurnInput = turnInput,

            Jump = JumpInput(),
            JumpHoldDuration = 1f,
            StuntA = false,
            StuntB = false,
            StuntC = false,

            Drift = false,
            BoostRam = false,
        };
    }

    private bool JumpInput()
    {
        Node currentNode;
        if (currentNodeIndex == 0)
        {
        currentNode = path.nodes[currentNodeIndex].GetComponent<Node>();
        }
        else 
        {
            currentNode = path.nodes[currentNodeIndex - 1].GetComponent<Node>();
        }
        if (currentNode != null && currentNode.isJumpNode && distanceToTarget < nodeReachThreshold)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public override ActorInputData GrabCurrentFrameInputs() => inputData;
}
}