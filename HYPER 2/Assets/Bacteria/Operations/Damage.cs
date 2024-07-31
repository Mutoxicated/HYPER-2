using UnityEngine;

public class Damage : MonoBehaviour {
    [SerializeField] private Bacteria bacteria;
    [SerializeField] private float damage;
    [SerializeField] private float step;

    private Option<IDamageable> damageable = Option<IDamageable>.None();

    private void OnEnable() {
        bacteria.onPopulationIncrease += () => {
            damage += step;
        };
        bacteria.onPopulationDecrease += () => {
            damage -= step;
        };
    }

    public void OnDisable() {
        damageable = Option<IDamageable>.None();
    }

    public void IntervalDamage() {
        if (damageable.IsNone()) {
            damageable = new Option<IDamageable>(bacteria.ImmuneSystem.GetComponent<IDamageable>());
        }
        int xp = damageable.Get().BacteriaDamage(bacteria.Type, damage);
        Player.IncrementXP(xp);
    }
}