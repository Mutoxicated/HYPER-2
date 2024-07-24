using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField] private Player player;
    [SerializeField] private health hp;

    private void OnCollisionEnter(Collision collision) {
        Destroy(collision.gameObject);
    }

    public void Die(Option<Stats> _) {
        player.DeadMode();
    }

    public void Damage(Stats _, float amount) {
        if (hp.Current < 0) { return; }
        hp.Current -= amount;
        if (hp.Current < 0) {
            Die(Option<Stats>.None());
        }
    }

    public void BacteriaDamage(Element bt, float amount) {
        if (hp.Current < 0) { return; }
        hp.Current -= amount;
        if (hp.Current < 0) {
            Die(Option<Stats>.None());
        }
    }

     public void ExplosionDamage(Explosion.Strength strength, float additive)
    {
        if (hp.Current < 0) { return; }
        
        hp.Current -= additive+(int)strength;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }
}
