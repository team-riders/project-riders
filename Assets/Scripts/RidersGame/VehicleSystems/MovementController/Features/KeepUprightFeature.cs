using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class KeepUprightFeature : IDataPipelineStep<Blackboard>
    {
        Vector3 m_VerticalReference = Vector3.up; // Reference vector for upright orientation
        private const float AirborneReorientationCoefficient = 3f; // Coefficient for airborne reorientation speed

        public Blackboard ProcessData(Blackboard blackboard)
        {
            Transform transform = blackboard.GetValue<Transform>("Transform");
            bool m_HasCollision = blackboard.GetValue<bool>("HasCollision");
            Vector3 m_LastCollisionNormal = blackboard.GetValue<Vector3>("LastCollisionNormal");
            float GroundPercent = blackboard.GetValue<float>("GroundPercent");
            Rigidbody Rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");
            Vector3 angularVelocity = blackboard.GetValue<Vector3>("IntentVelocity");

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

            return blackboard;
        }
    }
}