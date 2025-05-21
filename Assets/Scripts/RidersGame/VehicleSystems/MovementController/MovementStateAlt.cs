using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public abstract class MovementStateAlt : State
    {
        GameObject _parent;
        protected GameObject Parent => _parent;
        protected Blackboard _refBlackboard;
        protected DataPipleline<DataPipelineStep<Blackboard>, Blackboard> _featureQueue = new();
        public MovementStateAlt(string name) : base(name)
        {
            _refBlackboard = new Blackboard();
        }

        public Blackboard ReturnStateBlackboard()
        {
            return _refBlackboard;
        }

        public virtual void ComputePhysicsIntentions(Blackboard externalBlackboard = null)
        {
            _refBlackboard.AppendOrOverwrite(externalBlackboard);
            // Death happens here
            _featureQueue.Process(_refBlackboard);
        }

        public virtual void Setup(GameObject parent)
        {
            _parent = parent;
            _refBlackboard.Clear();
        }
    }
}