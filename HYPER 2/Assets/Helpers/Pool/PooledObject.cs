using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPool pool;

    public void CStart(ObjectPool pool) {
        this.pool = pool;
    }

    private void OnEnable() {
        transform.localPosition = Vector3.zero;
    }

    public void OnDeath() {
        pool.ReattachImmediate(gameObject);
    }
}
