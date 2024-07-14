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
    [SerializeField] private Interval DOTInterval;
    [SerializeField] private Resources.Arsenal arsenal;

    private health hp;
    private float DOT = .1f;

    public Stats Stats {
        get { return stats; }
    }

    public health HP {
        get { return hp; }
    }

    public Resources.Arsenal ArsenalType {
        get { return arsenal; }
    }

    public void CStart(Manager manager) {
        hp = stats.CStart(manager);

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void DOTIntervalDamage() {
        Damage(stats, DOT);
    }

    public void RestoreHP() {
        hp.Current = hp.Max;
    }

    public void Die(Option<Stats> _) {
        Destroy(gameObject);
    }

    public void Damage(Stats stats, float amount)
    {
        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }

    public void BacteriaDamage(BacteriaType bt, float amount) {
        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }
}
