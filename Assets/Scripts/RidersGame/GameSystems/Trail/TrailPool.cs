using System.Collections.Generic;
using UnityEngine;

public struct TrailPool
{
    private GameObject _prefabRef;
    private List<TrailCollision> _pool;

    public TrailPool(GameObject prefabRef, int initalCount)
    {
        _pool = new List<TrailCollision>();
        _prefabRef = prefabRef;
        GameObject temp;

        for (int i = 0; i < initalCount; i++)
        {
            temp = GameObject.Instantiate(prefabRef);
            _pool.Add(temp.GetComponent<TrailCollision>());
        }
    }

    public TrailCollision GetInactive()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeSelf) return _pool[i];
        }

        GameObject temp = GameObject.Instantiate(_prefabRef);
        TrailCollision tC = temp.GetComponent<TrailCollision>();
        _pool.Add(tC);
        return tC;
    }

    public void DeactivatePoints()
    {
        foreach (TrailCollision trailPoint in _pool)
        {
            //Reset the objects current rotation for reuse. 
            trailPoint.transform.rotation = Quaternion.identity;
            trailPoint.gameObject.SetActive(false);
        }
    }

}
