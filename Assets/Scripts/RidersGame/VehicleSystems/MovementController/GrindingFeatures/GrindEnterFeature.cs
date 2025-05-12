using RidersRuntime.VehicleSystem;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindEnterFeature : ConditionalDataPipelineStep<Blackboard>
    {
        public float feetOffsetY = 0.9f;
        public override Blackboard OnStep(Blackboard blackboard)
        {
            blackboard.Remove("Entered");

            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            GrindPath currentPath = blackboard.GetValue<GrindPath>("CurrentPath");

            Spline spline = currentPath.SplineContainer.Spline;
            Transform transform = rb.transform;

            blackboard.SetValue("CurrentSpeed", rb.linearVelocity.magnitude);

            rb.isKinematic = true;
            rb.useGravity = false;

            Vector3 currentPos = transform.position;
            float progress = GetStartProgress(currentPath, currentPos);

            // Progression direction (might be set at the start of the grind)
            spline.Evaluate(progress, out float3 position, out float3 tangent, out float3 normal);
            Vector3 vehicleDirection = transform.forward;
            float dot = Vector3.Dot(tangent, vehicleDirection);
            int progressDirection = dot > 0f ? 1 : -1;

            // We need to force the player onto the path

            float3 localPos = SplineUtility.EvaluatePosition(spline, progress);

            Vector3 worldPos = currentPath.SplineContainer.transform.TransformPoint((Vector3)localPos);
            Vector3 forward = currentPath.SplineContainer.transform.TransformDirection(tangent * progressDirection);
            Vector3 targetPos = worldPos + Vector3.up * feetOffsetY;

            Quaternion targetRot = Quaternion.LookRotation(forward);

            rb.position = targetPos;
            rb.rotation = targetRot;

            blackboard.SetValue("CurrentPath", currentPath);
            blackboard.SetValue("CurrentProgress", progress);
            blackboard.SetValue("Direction", progressDirection);

            return blackboard;
        }

        private float GetStartProgress(GrindPath path, Vector3 currentPosition)
        {
            Vector3 localPos = path.splineTransform.InverseTransformPoint(currentPosition);
            SplineUtility.GetNearestPoint(path.SplineContainer.Spline, (float3)localPos, out _, out float progress);
            return Mathf.Clamp01(progress);
        }

        public override bool IsConditionMet(Blackboard data)
        {
            // Check if the player is in the air and not on a grind path
            if (!data.ContainsKey("Entered"))
                return false;
            return data.GetValue<bool>("Entered");
        }

        public GrindEnterFeature(Func<bool> condition = null) : base(condition)
        {
        }
    }
}