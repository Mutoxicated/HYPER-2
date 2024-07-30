public interface IDamageable
{
    void Die(Option<Stats> stats);
    int Damage(Stats stats, float amount); // returns xp
    int BacteriaDamage(Element bt, float amount); 
    int ExplosionDamage(Explosion.Strength strength, float additive); 
}
