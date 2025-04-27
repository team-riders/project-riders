using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIInput : BaseInput
{

    private BaseVehicle baseVehicle;
    private Vector3 targetPosition;
    [SerializeField] private NodePath path;
    private int currentNodeIndex = 0;
    private float nodeReachThreshold = 3f;
    private float distanceToTarget;
    void Awake()
    {
        baseVehicle = GetComponent<BaseVehicle>();
    }
    
    ActorInputData inputData = new();
    void Update()
    {
        if (path == null || path.nodes.Length == 0) return;

        Transform targetNode = path.nodes[currentNodeIndex];
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
        float accelerate = distanceToTarget > 5f ? 1f : 0f;
        float brake = distanceToTarget < 3f ? 1f : 0f;
        
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
        Node currentNode = path.nodes[currentNodeIndex].GetComponent<Node>();
        if (currentNode != null && currentNode.isJumpNode && distanceToTarget < 2.5f)
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
