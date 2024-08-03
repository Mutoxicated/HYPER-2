using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsRedirect : MonoBehaviour, IStats
{
    [SerializeField] private Stats stats;
    [SerializeField] private Option<HitVisual> hitVisual = Option<HitVisual>.None();
    [SerializeField, Range(0.1f, 2f)] private float damageMultiplier = 1f;

    public Stats Convert()
    {
        return stats;
    }
    
    public void Die(Option<Stats> stats)
    {
        this.stats.Die(stats);
    }

    public void Heal(float amount) {
        stats.Heal(amount);
    }

    public int Damage(Stats stats, float amount)
    {
        if (hitVisual.IsSome())
            hitVisual.Get().OnHit(0.5f);
        return this.stats.Damage(stats, amount*damageMultiplier);
    }

    public int BacteriaDamage(Element bt, float amount)
    {
        if (hitVisual.IsSome())
            hitVisual.Get().OnHit(0.25f);
        return stats.BacteriaDamage(bt, amount*damageMultiplier);
    }

    public int ExplosionDamage(Explosion.Strength strength, float additive)
    {
        if (hitVisual.IsSome())
            hitVisual.Get().OnHit(0.75f);
        return stats.ExplosionDamage(strength, additive*damageMultiplier);
    }
}
