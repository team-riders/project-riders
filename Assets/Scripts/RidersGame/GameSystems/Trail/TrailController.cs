using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private TrailPool _pool;
    private bool _trailActive;
    private float _timeSinceLastStep;
    private int _currentSteps;
    private Queue<TrailCollision> _trailCollisionQueue;

    public bool active;
    public int allowedSteps;
    public float timeBetweenSteps;
    public float deactivationTime;
    public List<Vector3> stepList = new List<Vector3>();
    public GameObject trailGameobjectPrefab;

    void Start()
    {
        bool check = CheckTrailCanRun();
        if (!check) this.enabled = false;

        //Set up queue and object pool. 
        //--Feel free to replace this object pool with your own implementation. 

        _trailCollisionQueue = new Queue<TrailCollision>();
        _pool = new TrailPool(trailGameobjectPrefab, allowedSteps + 1);
    }

    private bool CheckTrailCanRun()
    {
        if (trailGameobjectPrefab == null)
        {
            Debug.Log("Trail Controller [" + gameObject.name + "] is missing a trail object.");
            return false;
        }

        GameObject tmp = GameObject.Instantiate(trailGameobjectPrefab);
        GameObject.Destroy(tmp);

        if (tmp.GetComponent<TrailCollision>() == null)
        {
            Debug.Log("Trail Controller [" + gameObject.name + "] is missing a trail object prefab has no TrailCollisionAdded.");
            return false;
        }

        return true;
    }

    void Update()
    {
        if (!_trailActive) return;

        _timeSinceLastStep += Time.deltaTime;

        if (_timeSinceLastStep > timeBetweenSteps)
        {
            _timeSinceLastStep -= timeBetweenSteps;
            AddStep();
        }
    }

    public void OnEnterFirstPlace()
    {
        //Activate the trail. 
        _trailActive = true;

        //Add the current player position to the object. 
        AddStep();
    }

    public void OnExitFirstPlace()
    {
        //Deactivate the trail.
        _trailActive = false;
        StartCoroutine(DeactivateTrailPoints(deactivationTime));
    }

    private void AddStep()
    {
        //Create the next trail step.
        
        Vector3 nextTrailPositon = transform.position;
        TrailCollision tColl;
        //Insert into the list. 
        // -- If list is longer than the cap, remove the first element
        // -- If list is less than the cap, add the new element without removal. 

        if (_trailCollisionQueue.Count > allowedSteps)
            tColl = _trailCollisionQueue.Dequeue();
        else tColl = _pool.GetInactive();

        tColl.transform.position = nextTrailPositon;
        tColl.Rotation = transform.rotation;
        _trailCollisionQueue.Enqueue(tColl);
    }

    private IEnumerator DeactivateTrailPoints(float deactivationTime)
    {
        yield return new WaitForSeconds(deactivationTime);
        _pool.DeactivatePoints();
    }
}
