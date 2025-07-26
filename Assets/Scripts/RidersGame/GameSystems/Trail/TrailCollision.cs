using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    /// <summary>
    /// Property shorthand for the transform.position of the object. 
    /// </summary>
    public Vector3 Position {
        get => transform.position;
        set => transform.position = value;
    }

    /// <summary>
    /// Property shorthand for the transform.rotation of the object. 
    /// </summary>
    public Quaternion Rotation {
        get => transform.rotation;
        set => transform.rotation = value;
    }

    /// <summary>
    /// Function used to setup an instance of the trail collider. 
    /// </summary>
    /// <param name="mask"> The layer mask used to filter raycasts. </param>
    /// <param name="minDistance"> The minimum distance range to apply to raycasts. </param>
    /// <param name="maxDistance"> The maximum distance range to apply to raycasts. </param>
    public void Setup(LayerMask mask, float minDistance, float maxDistance)
    {
        //Reset the last applied rotation. 
        //transform.rotation = Quaternion.identity;

        //Calculate the local downward normal. 
        Vector3 downwardNormal = -transform.up.normalized;
        
        //Raycast downward and get collisions. 
        Ray ray = new Ray(transform.position + (-transform.up.normalized), downwardNormal);
        RaycastHit[] hits = Physics.RaycastAll(ray, 30, mask);
        //Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        //If there are no collisions then return. 
        if (!(hits.Length > 0)) return;

        //Else filter for the closest collision point and cache it. 
        RaycastHit? closestHit = null;
        float dist = 99;

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance >= minDistance && hit.distance >= maxDistance && hit.distance < dist)
            {
                closestHit = hit;
                dist = hit.distance;
            }
        }

        //If there is a closest hit then...
        if (closestHit != null)
        {
            //Set the local rotation to the rotation of the surface. 
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.TransformDirection(Vector3.right), ((RaycastHit)closestHit).normal));
        }
    }
}
