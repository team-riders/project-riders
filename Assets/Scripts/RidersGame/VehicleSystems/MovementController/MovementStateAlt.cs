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

        public Blackboard ReturnStateBlackboard()
        {
            return _refBlackboard;
        }

        public virtual void ComputeIntention(Blackboard externalBlackboard = null)
        {
            _refBlackboard.AppendOrOverwrite(externalBlackboard);
            // TODO: Implement these passive features to the pipeline
            // Death happens here
            _featureQueue.Process(_refBlackboard);
        }

        public virtual void Setup()
        {
            _refBlackboard.Clear();
        }
    }
}