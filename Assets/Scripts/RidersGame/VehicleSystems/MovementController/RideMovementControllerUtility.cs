using RidersRuntime.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(Collider))]
    public partial class RideMovementController
    {
        float GroundPercent;
        bool m_HasCollision = false;
        Vector3 m_LastCollisionNormal = Vector3.zero;
        public List<WheelCollider> m_PhysicsWheels;
        Action ExecuteExternalPrechecks;
        GrindPath m_GrindPathTarget;

        public float GetMaxSpeed()
        {
            VehicleStats stats = m_VehicleStats._vehicleStats;
            return Mathf.Max(stats.TopSpeed, stats.ReverseSpeed);
        }

        public void SetCenterOfMass()
        {
            List<WheelCollider> wheelColliders = new();

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
            m_rigidbody.centerOfMass = transform.InverseTransformPoint(center);
        }

        public void Reset()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }

        public float LocalSpeed()
        {
            // ! Keep for VFX
            if (canMove)
            {
                float dot = Vector3.Dot(transform.forward, m_rigidbody.linearVelocity);
                if (Mathf.Abs(dot) > 0.1f)
                {
                    float speed = m_rigidbody.linearVelocity.magnitude;
                    return dot < 0 ? -(speed / powerupController.GetCurrentStats().ReverseSpeed) : (speed / powerupController.GetCurrentStats().TopSpeed);
                }
                return 0f;
            }
            else
            {
                // use this value to play kart sound when it is waiting the race start countdown.
                // change this
                return inputIntention.Accelerate;
            }
        }

        void GetGroundedPercent()
        {
            int groundedCount = 0;
            foreach (WheelCollider wheel in m_PhysicsWheels)
            {
                if (wheel.isGrounded && wheel.GetGroundHit(out WheelHit _))
                {
                    groundedCount++;
                }
            }

            float wheelCount = m_PhysicsWheels.Count;
            GroundPercent = (float)groundedCount / wheelCount;
        }

        #region Collision Detection -----------------------------------
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
        #endregion

        Blackboard SendBlackboard()
        {
            Blackboard blackboard = new();
            blackboard.SetValue("Rigidbody", m_rigidbody); // Don't need, can just set up in constructor
            blackboard.SetValue("VehicleStats", powerupController.ReturnComputedStats()); // Don't need, same as above
            blackboard.SetValue("InputData", inputIntention); // Might still need
            blackboard.SetValue("GroundPercent", GroundPercent);
            blackboard.SetValue("HasCollision", m_HasCollision);
            blackboard.SetValue("LastCollisionNormal", m_LastCollisionNormal);
            blackboard.SetValue("CurrentPath", m_GrindPathTarget);
            blackboard.SetValue("JumpIntention", JumpIntention);
            return blackboard;
        }

        public void SetGrindPath(GrindPath path)
        {
            if (grindingMovementState.CanEnterGrind())
                m_GrindPathTarget = path;
        }

    }
}