using RidersRuntime.VehicleSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersGame.VehicleSystem
{
    public class GrindEnterFeature : IDataPipelineStep<Blackboard>
    {
        public Blackboard ProcessData(Blackboard blackboard)
        {
            if (!blackboard.GetValue<bool>("Entered"))
                return blackboard;

            blackboard.SetValue("Entered", false);

            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            GrindPath currentPath = blackboard.GetValue<GrindPath>("CurrentPath");

            rb.isKinematic = true;
            rb.useGravity = false;

            Vector3 currentPos = rb.transform.position;

            float progress = GetStartProgress(currentPath, currentPos);

            blackboard.SetValue("CurrentPath", currentPath);
            blackboard.SetValue("CurrentProgress", progress);

            return blackboard;
        }

        private float GetStartProgress(GrindPath path, Vector3 currentPosition)
        {
            Vector3 localPos = path.splineTransform.InverseTransformPoint(currentPosition);
            SplineUtility.GetNearestPoint(path.SplineContainer.Spline, (float3)localPos, out _, out float progress);
            return Mathf.Clamp01(progress);
        }
    }
}