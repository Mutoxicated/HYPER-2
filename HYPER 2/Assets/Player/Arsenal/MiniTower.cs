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

    private health hp;
    private float DOT = .1f;

    public void CStart(Manager manager) {
        hp = stats.CStart(manager);

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }



    public void DOTIntervalDamage() {
        Damage(stats, DOT);
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
