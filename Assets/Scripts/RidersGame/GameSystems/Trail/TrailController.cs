using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to generate trails at runtime. 
/// </summary>
public class TrailController : MonoBehaviour
{
    /// <summary>
    /// Reference to the trail pool for this controller. 
    /// </summary>
    private TrailPool _pool;
    /// <summary>
    /// bool denoting if a trail is active. 
    /// </summary>
    private bool _trailActive;
    /// <summary>
    /// Boolean flag denoting the trail can create TrailCollisions. 
    /// </summary>
    private bool _createMarkers;
    /// <summary>
    /// The current queue of TrailCollision instances active in the trail. 
    /// </summary>
    private Queue<TrailCollision> _trailCollisionQueue;
    /// <summary>
    /// The last TrailCollision created in the trail. 
    /// </summary>
    private TrailCollision lastCollision;

    [Header("The max number of steps that the trail can have at a given time.")]
    public int maxAllowedSteps;
    [Header("The distance between each step.")]
    public float spawnDistance = 10.5F;
    [Header("The inital delay before deactivating the trail.")]
    public float delayBeforeDeactivation;
    [Header("The inital delay before deactivating each step in the trail.")]
    public float delayInbetweenDeactivations;
    [Header("The prefab to spawn at each step.")]
    public GameObject trailGameobjectPrefab;
    [Header("The collision layermask used to detect whether to spawn a trail object while on that surface.")]
    public LayerMask collisionLayerMask;
    [Header("The minimum allowed distance from the object for a surface.")]
    public float minSurfaceDistance;
    [Header("The maximum allowed distance from the object for a surface.")]
    public float maxSurfaceDistance;
    [Header("Should the trail spawn in the air (or when not actively colliding).")]
    public bool spawnInAir = false;

    /// <summary>
    /// Returns the current list of TrailCollision instances as an array. 
    /// </summary>
    public TrailCollision[] TrailCollisions => _trailCollisionQueue.ToArray();

    /// <summary>
    /// Returns the point at which the next trail marker should spawn. 
    /// </summary>
    private Vector3 NextTrailPosition => 
        transform.position + ((-transform.forward.normalized) * spawnDistance);

    /// <summary>
    /// Returns whether the player object has exceeded the allowed distance required to spawn the next TrailCollider. 
    /// </summary>
    private bool OutsideSpawnRange => 
        ((NextTrailPosition - lastCollision.Position).magnitude >= spawnDistance);

    /// <summary>
    /// Returns whether if the player is in air. 
    /// </summary>
    private bool InAir {
        get {
            //Cast in the down direction of the local transform. 
            Vector3 direction = -transform.up.normalized;
            Ray ray = new Ray(transform.position + (transform.up.normalized * (maxSurfaceDistance / 2)), direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, 30, collisionLayerMask);

            //If there is nothing within the hit range, then return false.  
            return (hits.Length > 0) ? false : true;
        }
    }

    /// <summary>
    /// Gets and returns an instance of the TrailCollider. 
    /// </summary>
    private TrailCollision GetTrailCollider {
        get {
            TrailCollision tColl; 
            // -- If list is longer than the cap, remove the first element
            // -- If list is less than the cap, add the new element without removal. 

            tColl = (_trailCollisionQueue.Count > maxAllowedSteps) ? 
                _trailCollisionQueue.Dequeue() : _pool.GetInactive();

            tColl.gameObject.SetActive(true);
            return tColl;
        }
    }

    private void Start()
    {
        //Check that the required components are provided. 
        if (!CheckTrailCanRun()) this.enabled = false;

        //Set up queue and object pool. 
        //--Feel free to replace this object pool with your own implementation. 

        //Create the trail queue and the trail pool. 
        _trailCollisionQueue = new Queue<TrailCollision>();
        _pool = new TrailPool(trailGameobjectPrefab, maxAllowedSteps + 5);
    }

    private bool CheckTrailCanRun()
    {
        if (trailGameobjectPrefab == null)
        {
            Debug.Log("Trail Controller [" + gameObject.name + "] is missing a trail object.");
            return false;
        }

        GameObject tmp = GameObject.Instantiate(trailGameobjectPrefab);

        if (tmp.GetComponent<TrailCollision>() == null)
        {
            Debug.Log("Trail Controller [" + gameObject.name + "] is missing a trail object prefab has no TrailCollisionAdded.");
            return false;
        }

        GameObject.Destroy(tmp);

        return true;
    }

    private void Update()
    {
        ///Debug Inputs for testing. 

        if (Input.GetKeyUp(KeyCode.End))
        {
            if (!_trailActive)
            {
                EnableTrail();
                return;
            }
            else if (_trailActive)
            {
                DisableTrail();
                return;
            }
        }

        //Do checks to see if can spawn. 
        if (!_trailActive || !OutsideSpawnRange || !_createMarkers) return;
        if (InAir && !spawnInAir) return;

        //Spawn the next TrailCollision. 
        AddStep();
    }

    /// <summary>
    /// Enable the trail. 
    /// </summary>
    public void EnableTrail()
    {
        //Activate the trail. 
        _createMarkers = _trailActive = true;

        //Add the current player position to the object. 
        AddStep();
    }

    /// <summary>
    /// Disable the trail. 
    /// </summary>
    public void DisableTrail()
    {
        //Stop the creation of further markers. 
        _createMarkers = false;
        
        //Deactivate the trail.
        StartCoroutine(DeactivateTrailPoints(delayBeforeDeactivation));
    }

    /// <summary>
    /// Add a TrailCollision step. 
    /// </summary>
    private void AddStep()
    {
        TrailCollision tColl = GetTrailCollider;

        //Set data to the TrailCollision. 
        tColl.Position = NextTrailPosition;
        tColl.Rotation = transform.rotation;
        tColl.Setup(collisionLayerMask, minSurfaceDistance, maxSurfaceDistance);

        //Add to the queue and cache a reference to the TrailCollision.
        _trailCollisionQueue.Enqueue(tColl);
        lastCollision = tColl;
    }

    /// <summary>
    /// Coroutine used to manage the deactivation of TrailCollisions. 
    /// </summary>
    /// <param name="deactivationTime"> Length of time that should pass prior to deactivation. </param>
    private IEnumerator DeactivateTrailPoints(float deactivationTime)
    {
        //Wait the provided deactivation time. 
        yield return new WaitForSeconds(deactivationTime);

        //Cache the current queue.
        TrailCollision[] trailCollisions = TrailCollisions;

        //Release the queue.
        _trailCollisionQueue.Clear();

        //For each instance of TrailCollision
        //-- Wait the delay
        //-- Then deactivate. 
        for (int i = 0; i < trailCollisions.Length; i++)
        {
            yield return new WaitForSeconds(delayInbetweenDeactivations);
            trailCollisions[i].gameObject.SetActive(false);
        }

        //_pool.DeactivatePoints();
        _trailActive = false;
    }
}
