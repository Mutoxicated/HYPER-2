using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum Element {
//     FIRE,
//     RADIATION
// }

public class MiniTower : MonoBehaviour, IDamageable
{
    [SerializeField] private Interval DOTInterval;
    private float health = 100f;
    private float DOT = .1f;

    private void CStart() {
        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void DOTIntervalDamage() {
        Damage(DOT);
    }

    public void Die() {
        //destroy
    }

    public void Damage(float amount)
    {
        health -= amount;
        if (health <= 0) {
            
        }
    }
}
