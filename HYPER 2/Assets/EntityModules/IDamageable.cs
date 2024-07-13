public interface IDamageable
{
    void Die(Option<Stats> stats);
    void Damage(Stats stats, float amount);    
    void BacteriaDamage(BacteriaType bt, float amount);
}
