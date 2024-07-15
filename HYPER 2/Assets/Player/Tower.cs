using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField] private Player player;
    [SerializeField] private float health = 200f;

    private void OnCollisionEnter(Collision collision) {
        Destroy(collision.gameObject);
    }

    public void Die(Option<Stats> _) {
        player.DeadMode();
    }

    public void Damage(Stats _, float amount) {
        if (health < 0) { return; }
        health -= amount;
        if (health < 0) {
            Die(Option<Stats>.None());
        }
    }

    public void BacteriaDamage(Element bt, float amount) {
          if (health < 0) { return; }
        health -= amount;
        if (health < 0) {
            Die(Option<Stats>.None());
        }
    }
}
