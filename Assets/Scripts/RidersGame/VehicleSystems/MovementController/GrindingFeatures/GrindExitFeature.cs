using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class GrindExitFeature : IDataPipelineStep<Blackboard>
    {
        const float KickoffSpeed = 10f;
        public Blackboard ProcessData(Blackboard blackboard)
        {
            Rigidbody rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");
            GrindPath grindPath = blackboard.GetValue<GrindPath>("CurrentPath");
            float progress = blackboard.GetValue<float>("CurrentProgress");

            bool inProgress = progress > 0.0f && progress < 1.0f;
            bool hasGrindPath = grindPath != null;



            if (!inProgress || !hasGrindPath)
            {
                blackboard.SetValue("IsGrinding", false);
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
                rigidbody.AddForce(new Vector3(0, 1, 0) * 10f, ForceMode.Impulse);
            }
            return blackboard;
        }
    }
}