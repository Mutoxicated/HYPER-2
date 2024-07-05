using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField] private Player player;
    [SerializeField] private float health = 200f;

    public void Die() {
        player.DeadMode();
    }

    public void Damage(float amount) {
        if (health < 0) { return; }
        health -= amount;
        if (health < 0) {
            Die();
        }
    }
}
