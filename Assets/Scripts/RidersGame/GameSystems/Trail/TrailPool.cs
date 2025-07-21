using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class defining object pool managements for the trail. 
/// </summary>
[System.Serializable]
public struct TrailPool
{
    /// <summary>
    /// The prefab used to fill the pool. 
    /// </summary>
    public GameObject _prefabRef;

    /// <summary>
    /// The list used to cache object references in the pool. 
    /// </summary>
    public List<TrailCollision> _pool;

    /// <summary>
    /// CTOR.
    /// </summary>
    /// <param name="prefabRef"> The prefab type the pool should manage. </param>
    /// <param name="initalCount"> The inital number of instances to create in the pool. </param>
    public TrailPool(GameObject prefabRef, int initalCount)
    {
        //Create a list for referencing objects in the pool. 
        _pool = new List<TrailCollision>();
        //Set the reference. 
        _prefabRef = prefabRef;

        //Create the instances required inside the pool. 
        for (int i = 0; i < initalCount; i++)
        {
            _pool.Add(GameObject.Instantiate(prefabRef).GetComponent<TrailCollision>());
        }
    }

    /// <summary>
    /// Retrive an object from the pool. 
    /// </summary>
    /// <returns> A TrailCollision instance from the pool. </returns>
    public TrailCollision GetInactive()
    {
        ///Search the pool for an inactive instance. 
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeSelf) return _pool[i];
        }

        //If there was no inactive instance then generate a new one in the pool and return it. 
        GameObject temp = GameObject.Instantiate(_prefabRef);
        TrailCollision tC = temp.GetComponent<TrailCollision>();
        _pool.Add(tC);
        return tC;
    }
}
