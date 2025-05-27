using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour {
    [SerializeField] private Bacteria bacteria;
    [SerializeField] private float interval;
    [SerializeField] private float damage;
    [SerializeField] private float step;

    private Option<IStats> stats = Option<IStats>.None();
    private float time = 0f;

    private void Start() {
        bacteria.onPopulationIncrease += () => {
            damage += step;
        };
        bacteria.onPopulationDecrease += () => {
            damage -= step;
        };
    }

    private void Update() {
        time += Time.deltaTime;
        if (time >= interval) {
            IntervalDamage();
            time = 0f;
        }
    }


    public void OnDisable() {
        stats = Option<IStats>.None();
    }

    public void IntervalDamage() {
        if (stats.IsNone()) {
            stats = new Option<IStats>(bacteria.ImmuneSystem.GetComponent<IStats>());
        }
        var converted = stats.Get().Convert();
        int xp = stats.Get().BacteriaDamage(bacteria.Type, damage*Mathf.InverseLerp(converted[Stat.MAX_HEALTH], 0, converted.HP));
        if (bacteria.Stats.StatSide == Stats.Side.PLAYER) {
            bacteria.Stats.IncXP(xp/2);
            Player.self.IncrementXP(xp);
        }
    }
}