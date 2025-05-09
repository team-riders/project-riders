namespace RidersRuntime.VehicleSystem
{
    public class AirborneMovementState : MovementStateAlt
    {
        public AirborneMovementState() : base("Airborne Movement State")
        {
            _featureQueue = new(new()
            {
                //new PowerupsFeature(),
                new AccelerationFeature(),
                new SteerTurnFeature(),
                new ScaleToFixedDeltaTime(),
                new CoastingFeature(),
                new FallFasterInAirFeature(),
                new KeepUprightFeature(),
                //new BoostFeature(),
            });
        }
    }
}