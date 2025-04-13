using System.Collections.Generic;
using UnityEngine;
using RidersCore.Data;

namespace RidersCore
{
    public class BaseVehicle : MonoBehaviour
    {
        // powerup stuff, used for speed boost
        [System.Serializable]
        public class StatPowerup
        {
            public VehicleStats modifiers;
            public string PowerUpID;
            public float ElapsedTime;
            public float MaxTime;
        }

        [Header("Vehicle Type")]
        public VehicleType VehicleType;

        public Rigidbody Rigidbody { get; private set; }
        public ActorInputData Input { get; private set; }
        public float AirPercent { get; private set; }
        public float GroundPercent { get; private set; }

        // figure out methods we need, refer to ArcadeKart.cs from karting microgame as startpoint

        public VehicleStats baseStats = new VehicleStats
        {
            TopSpeed = 50f,
            Acceleration = 10f,
            AccelerationCurve = 0.4f,
            Braking = 10f,
            ReverseAcceleration = 5f,
            ReverseSpeed = 5f,
            Steer = 5f,
            CoastingDrag = 4f,
            Grip = .95f,
            AddedGravity = 1f,
            BoostTopSpeed = 80f,
            BoostAccel = 15f,
        };

        // list is created, wheels are not until child classes
        [Header("Vehicle Visual")]
        public List<WheelCollider> m_VisualWheels;

        [Header("Vehicle Physics")]
        [Range(0.0f, 20.0f), Tooltip("Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane.")]
        public float AirborneReorientationCoefficient = 3.0f;

        //brought old drifting stuff back in to try and figure out what's not letting the vehicle move
        [Header("Drifting")]
        [Range(0.01f, 1.0f), Tooltip("The grip value when drifting.")]
        public float DriftGrip = 0.4f;
        [Range(0.0f, 10.0f), Tooltip("Additional steer when the kart is drifting.")]
        public float DriftAdditionalSteer = 5.0f;
        [Range(1.0f, 30.0f), Tooltip("The higher the angle, the easier it is to regain full grip.")]
        public float MinAngleToFinishDrift = 10.0f;
        [Range(0.01f, 0.99f), Tooltip("Mininum speed percentage to switch back to full grip.")]
        public float MinSpeedPercentToFinishDrift = 0.5f;
        [Range(1.0f, 20.0f), Tooltip("The higher the value, the easier it is to control the drift steering.")]
        public float DriftControl = 10.0f;
        [Range(0.0f, 20.0f), Tooltip("The lower the value, the longer the drift will last without trying to control it by steering.")]
        public float DriftDampening = 10.0f;

        //VFX for wheels, should be able to be used anywhere
        [Header("VFX")]
        [Tooltip("VFX that will be placed on the wheels when drifting.")]
        public ParticleSystem DriftSparkVFX;
        [Range(0.0f, 0.2f), Tooltip("Offset to displace the VFX to the side.")]
        public float DriftSparkHorizontalOffset = 0.1f;
        [Range(0.0f, 90.0f), Tooltip("Angle to rotate the VFX.")]
        public float DriftSparkRotation = 17.0f;
        [Tooltip("VFX that will be placed on the wheels when drifting.")]
        public GameObject DriftTrailPrefab;
        [Range(-0.1f, 0.1f), Tooltip("Vertical to move the trails up or down and ensure they are above the ground.")]
        public float DriftTrailVerticalOffset;
        [Tooltip("VFX that will spawn upon landing, after a jump.")]
        public GameObject JumpVFX;
        [Tooltip("VFX that is spawn on the nozzles of the kart.")]
        public GameObject NozzleVFX;
        [Tooltip("List of the kart's nozzles.")]
        public List<Transform> Nozzles;

        //completely different drift implementation compared to kart microgame

        //don't need suspension

        //physical wheels go into child classes

        [Tooltip("Which layers the wheels will detect.")]
        public LayerMask GroundLayers = Physics.DefaultRaycastLayers;

        // the input sources that can control the kart
        IInput[] m_Inputs;

        const float k_NullInput = 0.01f;
        const float k_NullSpeed = 0.01f;
        Vector3 m_VerticalReference = Vector3.up;

        // Drift params
        // CHANGE LATER
        public bool WantsToDrift { get; private set; } = false;
        public bool IsDrifting { get; private set; } = false;
        float m_CurrentGrip = 1.0f;
        float m_DriftTurningPower = 0.0f;
        float m_PreviousGroundPercent = 1.0f;
        readonly List<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();
        readonly List<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>();

        // can the kart move?
        bool m_CanMove = true;
        List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>();
        VehicleStats m_FinalStats;

        Quaternion m_LastValidRotation;
        Vector3 m_LastValidPosition;
        Vector3 m_LastCollisionNormal;
        bool m_HasCollision;
        bool m_InAir = false;

        //jumping stuff
        [Header("Jump")]
        [Range(0.1f, 1.0f), Tooltip("Stores charge amount for jumping on ramps; helps determine ramp jump height, directly correlates to trick speed")]
        float JumpCharge;
        [Tooltip("Stores jump force")]
        public float JumpForce = 100.0f;

        // methods
        public void AddPowerup(StatPowerup statPowerup) => m_ActivePowerupList.Add(statPowerup);
        public void SetCanMove(bool move) => m_CanMove = move;
        public float GetMaxSpeed() => Mathf.Max(m_FinalStats.TopSpeed, m_FinalStats.ReverseSpeed);

        //// seems like adding wheels to m_VisualWheels is through unity itself and not programmatically
        //public void AddWheelToVisual(WheelCollider wheelCollider)
        //{
        //    m_VisualWheels.Add(wheelCollider);
        //}

        private void ActivateDriftVFX(bool active)
        {
            foreach (var vfx in m_DriftSparkInstances)
            {
                if (active && vfx.wheel.GetGroundHit(out WheelHit hit))
                {
                    if (!vfx.sparks.isPlaying)
                        vfx.sparks.Play();
                }
                else
                {
                    if (vfx.sparks.isPlaying)
                        vfx.sparks.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }

            }

            foreach (var trail in m_DriftTrailInstances)
                trail.Item3.emitting = active && trail.wheel.GetGroundHit(out WheelHit hit);
        }

        private void UpdateDriftVFXOrientation()
        {
            foreach (var vfx in m_DriftSparkInstances)
            {
                vfx.sparks.transform.position = vfx.wheel.transform.position - (vfx.wheel.radius * Vector3.up) + (DriftTrailVerticalOffset * Vector3.up) + (transform.right * vfx.horizontalOffset);
                vfx.sparks.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, vfx.rotation);
            }

            foreach (var trail in m_DriftTrailInstances)
            {
                trail.trailRoot.transform.position = trail.wheel.transform.position - (trail.wheel.radius * Vector3.up) + (DriftTrailVerticalOffset * Vector3.up);
                trail.trailRoot.transform.rotation = transform.rotation;
            }
        }

        //make virtual?
        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            m_Inputs = GetComponents<IInput>();

            m_CurrentGrip = baseStats.Grip;

            SetCenterOfMass();

            // previously initialised in karting microgame by FixedUpdates() calling TickPowerups()
            //m_FinalStats = baseStats;

            // add to child classes instead

            //if (DriftSparkVFX != null)
            //{
            //    AddSparkToWheel(RearLeftWheel, -DriftSparkHorizontalOffset, -DriftSparkRotation);
            //    AddSparkToWheel(RearRightWheel, DriftSparkHorizontalOffset, DriftSparkRotation);
            //}

            //if (DriftTrailPrefab != null)
            //{
            //    AddTrailToWheel(RearLeftWheel);
            //    AddTrailToWheel(RearRightWheel);
            //}
        }

        ////add to child classes
        //void AddTrailToWheel(WheelCollider wheel)
        //{
        //    GameObject trailRoot = Instantiate(DriftTrailPrefab, gameObject.transform, false);
        //    TrailRenderer trail = trailRoot.GetComponentInChildren<TrailRenderer>();
        //    trail.emitting = false;
        //    m_DriftTrailInstances.Add((trailRoot, wheel, trail));
        //}

        //void AddSparkToWheel(WheelCollider wheel, float horizontalOffset, float rotation)
        //{
        //    GameObject vfx = Instantiate(DriftSparkVFX.gameObject, wheel.transform, false);
        //    ParticleSystem spark = vfx.GetComponent<ParticleSystem>();
        //    spark.Stop();
        //    m_DriftSparkInstances.Add((wheel, horizontalOffset, -rotation, spark));
        //}

        //make virtual?
        void FixedUpdate()
        {
            GatherInputs();

            // maybe later
            // apply our powerups to create our finalStats
            TickPowerups();

            // apply our physics properties

            int groundedCount = 0;
            foreach (WheelCollider o in m_VisualWheels)
            {
                if (o.isGrounded && o.GetGroundHit(out WheelHit hit))
                {
                    groundedCount++;
                }
            }
            //if (FrontLeftWheel.isGrounded && FrontLeftWheel.GetGroundHit(out WheelHit hit))
            //    groundedCount++;
            //if (FrontRightWheel.isGrounded && FrontRightWheel.GetGroundHit(out hit))
            //    groundedCount++;
            //if (RearLeftWheel.isGrounded && RearLeftWheel.GetGroundHit(out hit))
            //    groundedCount++;
            //if (RearRightWheel.isGrounded && RearRightWheel.GetGroundHit(out hit))
            //    groundedCount++;

            // calculate how grounded and airborne we are
            float wheelCount = m_VisualWheels.Count;
            GroundPercent = (float)groundedCount / wheelCount;
            AirPercent = 1 - GroundPercent;

            // apply vehicle physics
            if (m_CanMove)
            {
                //Debug.Log("Input.Jump.WasReleasedThisFrame: " + Input.Jump);
                MoveVehicle(Input.Accelerate == 1, Input.Brake == 1, Input.TurnInput, Input.Jump, Input.JumpHoldDuration);
            }
            GroundAirbourne();

            m_PreviousGroundPercent = GroundPercent;

            UpdateDriftVFXOrientation();
        }

        void SetCenterOfMass()
        {
            List<WheelCollider> wheelColliders = new List<WheelCollider>();

            // Get all WheelColliders under BoardVehicle
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                WheelCollider wc = child.GetComponent<WheelCollider>();
                if (wc != null)
                {
                    wheelColliders.Add(wc);
                }
            }

            if (wheelColliders.Count == 0)
            {
                //Debug.LogError("No WheelColliders found!");
                return;
            }

            // Calculate the average position of all WheelColliders
            Vector3 center = Vector3.zero;
            foreach (WheelCollider wc in wheelColliders)
            {
                center += wc.transform.position;
            }
            center /= wheelColliders.Count;

            Rigidbody.centerOfMass = transform.InverseTransformPoint(center);
        }

        void GatherInputs()
        {
            // reset input
            Input = new ActorInputData();
            WantsToDrift = false;

            // gather nonzero input from our sources
            for (int i = 0; i < m_Inputs.Length; i++)
            {
                Input = m_Inputs[i].GrabCurrentFrameInputs();
                WantsToDrift = Input.Brake == 1 && Vector3.Dot(Rigidbody.linearVelocity, transform.forward) > 0.0f;
            }
        }

        // ignore for now, delete if we decide we don't want powerups
        void TickPowerups()
        {
            // remove all elapsed powerups
            m_ActivePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });

            // zero out powerups before we add them all up
            var powerups = new VehicleStats();

            // add up all our powerups
            for (int i = 0; i < m_ActivePowerupList.Count; i++)
            {
                var p = m_ActivePowerupList[i];

                // add elapsed time
                p.ElapsedTime += Time.fixedDeltaTime;

                // add up the powerups
                powerups += p.modifiers;
            }

            // add powerups to our final stats
            m_FinalStats = baseStats + powerups;

            // clamp values in finalstats
            m_FinalStats.Grip = Mathf.Clamp(m_FinalStats.Grip, 0, 1);
        }

        void GroundAirbourne()
        {
            // while in the air, fall faster
            if (AirPercent >= 1)
            {
                Rigidbody.linearVelocity += Physics.gravity * Time.fixedDeltaTime * m_FinalStats.AddedGravity;
            }
        }

        public void Reset()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }

        public float LocalSpeed()
        {
            if (m_CanMove)
            {
                float dot = Vector3.Dot(transform.forward, Rigidbody.linearVelocity);
                if (Mathf.Abs(dot) > 0.1f)
                {
                    float speed = Rigidbody.linearVelocity.magnitude;
                    return dot < 0 ? -(speed / m_FinalStats.ReverseSpeed) : (speed / m_FinalStats.TopSpeed);
                }
                return 0f;
            }
            else
            {
                // use this value to play kart sound when it is waiting the race start countdown.
                // change this
                return Input.Accelerate;
            }
        }

        // change later
        void OnCollisionEnter(Collision collision) => m_HasCollision = true;
        void OnCollisionExit(Collision collision) => m_HasCollision = false;

        void OnCollisionStay(Collision collision)
        {
            m_HasCollision = true;
            m_LastCollisionNormal = Vector3.zero;
            float dot = -1.0f;

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > dot)
                    m_LastCollisionNormal = contact.normal;
            }
        }

        //make virtual?
        void MoveVehicle(bool accelerate, bool brake, float turnInput, bool jump, float jumpHold)
        {
            float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);
            //Debug.Log("accelInput: " + accelInput);

            // manual acceleration curve coefficient scalar
            float accelerationCurveCoeff = 5;
            Vector3 localVel = transform.InverseTransformVector(Rigidbody.linearVelocity);

            bool accelDirectionIsFwd = accelInput >= 0;
            //Debug.Log("accelDirectionIsFwd: " + accelDirectionIsFwd);
            bool localVelDirectionIsFwd = localVel.z >= 0;

            // use the max speed for the direction we are going--forward or reverse.
            float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
            float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;
            //Debug.Log("accelPower: " + accelPower);

            float currentSpeed = Rigidbody.linearVelocity.magnitude;
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

            // if we are braking (moving reverse to where we are going)
            // use the braking acceleration instead
            float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

            float finalAcceleration = finalAccelPower * accelRamp;
            //Debug.Log(finalAcceleration);

            // apply inputs to forward/backward
            float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * m_FinalStats.Steer;

            Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
            Vector3 fwd = turnAngle * transform.forward;
            Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);

            // forward movement
            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

            // if over max speed, cannot accelerate faster.
            if (wasOverMaxSpeed && !isBraking)
                movement *= 0.0f;

            Vector3 newVelocity = Rigidbody.linearVelocity + movement * Time.fixedDeltaTime;
            newVelocity.y = Rigidbody.linearVelocity.y;

            //  clamp max speed if we are on ground
            if (GroundPercent > 0.0f && !wasOverMaxSpeed)
            {
                newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
            }

            // coasting is when we aren't touching accelerate
            if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
            {
                newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, Rigidbody.linearVelocity.y, 0), Time.fixedDeltaTime * m_FinalStats.CoastingDrag);
            }

            Rigidbody.linearVelocity = newVelocity;

            // Drift
            if (GroundPercent > 0.0f)
            {
                if (m_InAir)
                {
                    m_InAir = false;
                    //Instantiate(JumpVFX, transform.position, Quaternion.identity);
                }

                // manual angular velocity coefficient
                float angularVelocitySteering = 0.4f;
                float angularVelocitySmoothSpeed = 20f;

                // turning is reversed if we're going in reverse and pressing reverse
                if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                    angularVelocitySteering *= -1.0f;

                var angularVel = Rigidbody.angularVelocity;

                // move the Y angular velocity towards our target
                angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.fixedDeltaTime * angularVelocitySmoothSpeed);

                // apply the angular velocity
                Rigidbody.angularVelocity = angularVel;

                // rotate rigidbody's velocity as well to generate immediate velocity redirection
                // manual velocity steering coefficient
                float velocitySteering = 25f;

                // drifting, change later
                // If the karts lands with a forward not in the velocity direction, we start the drift
                if (GroundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
                {
                    Vector3 flattenVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, m_VerticalReference).normalized;
                    if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad))
                    {
                        IsDrifting = true;
                        m_CurrentGrip = DriftGrip;
                        m_DriftTurningPower = 0.0f;
                    }
                }

                // Drift Management
                if (!IsDrifting)
                {
                    if ((WantsToDrift || isBraking) && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
                    {
                        IsDrifting = true;
                        m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * DriftAdditionalSteer);
                        m_CurrentGrip = DriftGrip;

                        ActivateDriftVFX(true);
                    }
                }

                if (IsDrifting)
                {
                    float turnInputAbs = Mathf.Abs(turnInput);
                    if (turnInputAbs < k_NullInput)
                        m_DriftTurningPower = Mathf.MoveTowards(m_DriftTurningPower, 0.0f, Mathf.Clamp01(DriftDampening * Time.fixedDeltaTime));

                    // Update the turning power based on input
                    float driftMaxSteerValue = m_FinalStats.Steer + DriftAdditionalSteer;
                    m_DriftTurningPower = Mathf.Clamp(m_DriftTurningPower + (turnInput * Mathf.Clamp01(DriftControl * Time.fixedDeltaTime)), -driftMaxSteerValue, driftMaxSteerValue);

                    bool facingVelocity = Vector3.Dot(Rigidbody.linearVelocity.normalized, transform.forward * Mathf.Sign(accelInput)) > Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad);

                    bool canEndDrift = true;
                    if (isBraking)
                        canEndDrift = false;
                    else if (!facingVelocity)
                        canEndDrift = false;
                    else if (turnInputAbs >= k_NullInput && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
                        canEndDrift = false;

                    if (canEndDrift || currentSpeed < k_NullSpeed)
                    {
                        // No Input, and car aligned with speed direction => Stop the drift
                        IsDrifting = false;
                        m_CurrentGrip = m_FinalStats.Grip;
                    }

                }

                // rotate our velocity based on current steer value
                Rigidbody.linearVelocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime, transform.up) * Rigidbody.linearVelocity;
            }
            else
            {
                m_InAir = true;
            }

            bool validPosition = false;
            if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out RaycastHit hit, 3.0f, 1 << 9 | 1 << 10 | 1 << 11)) // Layer: ground (9) / Environment(10) / Track (11)
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hit.normal.y) ? m_LastCollisionNormal : hit.normal;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime * (GroundPercent > 0.0f ? 10.0f : 1.0f)));    // Blend faster if on ground
            }
            else
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime));
            }

            validPosition = GroundPercent > 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;

            // Airborne / Half on ground management
            if (GroundPercent < 0.7f)
            {
                Rigidbody.angularVelocity = new Vector3(0.0f, Rigidbody.angularVelocity.y * 0.98f, 0.0f);
                Vector3 finalOrientationDirection = Vector3.ProjectOnPlane(transform.forward, m_VerticalReference);
                finalOrientationDirection.Normalize();
                if (finalOrientationDirection.sqrMagnitude > 0.0f)
                {
                    Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime)));
                }
            }
            else if (validPosition)
            {
                m_LastValidPosition = transform.position;
                m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
            }

            //jump management
            // should be satisfactory until we add ramps
            if (jump && GroundPercent > 0.0f)
            {
                Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                JumpCharge = Mathf.Clamp(jumpHold / 120f, 0.1f, 1.0f);
                Debug.Log("JumpCharge: " + JumpCharge);
            }

            ActivateDriftVFX(IsDrifting && GroundPercent > 0.0f);
        }
    }
}