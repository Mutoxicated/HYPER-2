public interface IDamageable
{
    void Die(Option<Stats> stats);
    void Damage(Stats stats, float amount);    
    void BacteriaDamage(Element bt, float amount);
    void ExplosionDamage(Explosion.Strength strength, float additive);
}
