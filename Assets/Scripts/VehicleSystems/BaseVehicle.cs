using UnityEngine;

public class BaseVehicle : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        // figure out what stats we need
        // currently just copy+pasting from microgame
        [Header("Movement Settings")]
        [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
        public float TopSpeed;

        [Tooltip("How quickly the kart reaches top speed.")]
        public float Acceleration;

        [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
        public float ReverseSpeed;

        [Tooltip("How quickly the kart reaches top speed, when moving backward.")]
        public float ReverseAcceleration;

        [Tooltip("How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
        [Range(0.2f, 1)]
        public float AccelerationCurve;

        [Tooltip("How quickly the kart slows down when the brake is applied.")]
        public float Braking;

        [Tooltip("How quickly the kart will reach a full stop when no inputs are made.")]
        public float CoastingDrag;

        [Range(0.0f, 1.0f)]
        [Tooltip("The amount of side-to-side friction.")]
        public float Grip;

        [Tooltip("How tightly the kart can turn left or right.")]
        public float Steer;

        [Tooltip("Additional gravity for when the kart is in the air.")]
        public float AddedGravity;
    }

    public Rigidbody Rigidbody {  get; private set; }
    public InputData Input {  get; private set; }
    public float AirPercent { get; private set; }
    public float GroundPercent { get; private set; }

    // figure out methods we need, refer to ArcadeKart.cs from karting microgame as startpoint

    public BaseVehicle.Stats baseStats = new BaseVehicle.Stats
    {
        TopSpeed = 50f,
        Acceleration = 10f,
        AccelerationCurve = 4f,
        Braking = 10f,
        ReverseAcceleration = 5f,
        ReverseSpeed = 5f,
        Steer = 5f,
        CoastingDrag = 4f,
        Grip = .95f,
        AddedGravity = 1f,
    };
}
