public interface IDamageable
{
    void Die(Option<Stats> stats);
    int Damage(Stats stats, float amount); // returns true when this damage caused it to die
    int BacteriaDamage(Element bt, float amount); // returns true when this damage caused it to die
    int ExplosionDamage(Explosion.Strength strength, float additive); // returns true when this damage caused it to die
}
