using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPool pool;

    public void CStart(ObjectPool pool) {
        this.pool = pool;
    }

    public void OnDisable() {
        pool.ReattachImmediate(gameObject);
    }
}
