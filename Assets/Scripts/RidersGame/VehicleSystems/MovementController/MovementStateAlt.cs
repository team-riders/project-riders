

using System;
using UnityEngine;
using RidersRuntime.Input;
using RidersRuntime.Data;

namespace RidersRuntime.VehicleSystem
{
    public abstract class MovementStateAlt : State
    {
        protected Blackboard _refBlackboard;
        protected DataPipleline<IDataPipelineStep<Blackboard>, Blackboard> _featureQueue = new();
        public MovementStateAlt(string name) : base(name)
        {
            _refBlackboard = new Blackboard();
        }

        public void PassBB(Blackboard blackboard)
        {
            _refBlackboard = blackboard;
        }

        public virtual void ComputeIntention(ActorInputData input, out Vector3 intent_velocity, out Vector3 intent_rotation)
        {
            // TODO: Implement these passive features to the pipeline
            _refBlackboard.SetValue("InputData", input);
            _refBlackboard.SetValue("GroundPercent", 1.0f);
            _refBlackboard.SetValue("HasCollision", false);

            // _refBlackboard.SetValue("IntentVelocity", Vector3.zero);
            // _refBlackboard.SetValue("IntentRotation", Vector3.zero);

            // Death happens here
            _featureQueue.Process(_refBlackboard);

            // Everybody will have access to RB anyways and it serves as an intention because 
            // it doesn't get executed until FixedUpdate is complete.

            // intent_velocity = _refBlackboard.GetValue<Vector3>("IntentVelocity");
            // intent_rotation = _refBlackboard.GetValue<Vector3>("IntentRotation");

            // Does nothing don't worry about it
            intent_velocity = Vector3.zero;
            intent_rotation = Vector3.zero;
        }

        public virtual void Setup(VehicleStats stats, Rigidbody rb)
        {
            _refBlackboard.Clear();
            _refBlackboard.SetValue("VehicleStats", stats);
            _refBlackboard.SetValue("Rigidbody", rb);
        }
    }
}