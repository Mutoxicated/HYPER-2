using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum Element {
//     FIRE,
//     RADIATION
// }


[Serializable]
public class health {
    [SerializeField] private float max;
    [HideInInspector] public float Current;

    public float Max {
        get { return max; }
    }

    public health(float max) {
        this.max = max;
        Current = max;
    }
}

public class MiniTower : MonoBehaviour, IDamageable
{
    [SerializeField] private Stats stats;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Interval DOTInterval;
    [SerializeField] private Interval ShootInterval;
    [SerializeField] private Shoot shoot;

    public health hp = new health(100f);
    private float DOT = .1f;

    public void CStart() {
        stats.CStart();
        gameObject.SetActive(true);
        DOTInterval.Enable();
        ShootInterval.AddEvent(shootBullet);
        ShootInterval.Enable();
    }

    private void shootBullet() {
        shoot.Action(firePoint.position, b => b.CStart(firePoint.rotation));
    }

    public void DOTIntervalDamage() {
        Damage(DOT);
    }

    public void Die() {
        //destroy
    }

    public void Damage(float amount)
    {
        hp.Current -= amount;
        if (hp.Current <= 0) {
            
        }
    }
}
