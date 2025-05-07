using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RidersRuntime.VehicleSystem;

namespace RidersRuntime.Input
{
public class AIInput : BaseInput
{
    InputActionAsset m_playerInput;
    [SerializeField] private NodePath path;
    [SerializeField] private float nodeReachThreshold = 3f;
    [SerializeField] private float nodeCloseThreshold = 2f;
    [SerializeField] private float nodeAccelerateThreshold = 3.5f;
    private BaseVehicle baseVehicle;
    private Vector3 targetPosition;
   
    private int currentNodeIndex = 0;
    public float distanceToTarget;
    public Transform targetNode;
    private Node currentJumpNode;
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
            currentNodeIndex = (currentNodeIndex + 1) % path.nodes.Length;
        
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
        float accelerate = distanceToTarget > nodeAccelerateThreshold ? 1f : 0f;
        float brake = distanceToTarget < nodeCloseThreshold ? 1f : 0f;
        
        return new ActorInputData
        {
            Accelerate = accelerate,
            Brake = brake,
            TurnInput = turnInput,

            Jump = JumpInput(),
            JumpHoldDuration = currentJumpNode.jumpHoldDuration,
            StuntA = false,
            StuntB = false,
            StuntC = false,

            Drift = false,
            BoostRam = false,
        };
    }

    private bool JumpInput()
    {
        currentJumpNode = CorrectNodeIndex();

        if (currentJumpNode != null && currentJumpNode.isJumpNode && distanceToTarget < nodeReachThreshold)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    private Node CorrectNodeIndex()
    {
        if (currentNodeIndex == 0)
        {
            currentJumpNode = path.nodes[currentNodeIndex].GetComponent<Node>();
        }
        else 
        {
            currentJumpNode = path.nodes[currentNodeIndex - 1].GetComponent<Node>();
        }
        return currentJumpNode;
    }
    public override ActorInputData GrabCurrentFrameInputs() => inputData;
}
}