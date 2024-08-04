using UnityEngine;

public class Damage : MonoBehaviour {
    [SerializeField] private Bacteria bacteria;
    [SerializeField] private float damage;
    [SerializeField] private float step;

    private Option<IDamageable> damageable = Option<IDamageable>.None();

    private void Start() {
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
        if (bacteria.Stats.StatSide == Stats.Side.PLAYER) {
            bacteria.Stats.IncXP(xp/2);
            Player.IncrementXP(xp);
        }
    }
}