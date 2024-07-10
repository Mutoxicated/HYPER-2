using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stat;

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
    [SerializeField] private GameObject shootSystem;
    [SerializeField] private Interval ShootInterval;
    [SerializeField] private Shoot shoot;

    private health hp;
    private float DOT = .1f;

    private Manager manager;

    public void CStart(Manager manager) {
        this.manager = manager;
        hp = stats.CStart(manager);
        shootSystem.SetActive(true);

        gameObject.SetActive(true);
        DOTInterval.Enable();
        ShootInterval.AddEvent(shootBullet);
        ShootInterval.Enable();
    }

    private void shootBullet() {
        if (stats.TargetIsValid())
            shoot.Action(firePoint.position, b => b.CStart(manager, firePoint.rotation));
    }

    public void DOTIntervalDamage() {
        Damage(stats, DOT);
    }

    public void Die(Stats _) {
        Destroy(gameObject);
    }

    public void Damage(Stats stats, float amount)
    {
        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(stats);
        }
    }
}
