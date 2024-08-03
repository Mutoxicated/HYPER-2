using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicPools : MonoBehaviour
{
    private Dictionary<PoolID, ObjectPool> pools = new Dictionary<PoolID, ObjectPool>();
    [SerializeField] private List<ObjectPool> poolList = new List<ObjectPool>();

    public ObjectPool this[PoolID id] {
        get { return pools[id]; }
    }

    public void DStart()
    {
        foreach (var pool in poolList) {
            pools.Add(pool.GetID(), pool);
        }
    }

    public void RetrieveAllObjectsToPools(){
        foreach (var poolname in pools.Keys){
            pools[poolname].ReattachAll();
        }
    }

    public bool PoolsExists(PoolID ID)
    {
        return pools.ContainsKey(ID);
    }
}