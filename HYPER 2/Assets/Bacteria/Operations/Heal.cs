using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] private Bacteria bacteria;
    [SerializeField] private float heal;
    [SerializeField] private float step;

    private Option<IDamageable> damageable = Option<IDamageable>.None();

    private void Start() {
        bacteria.onPopulationIncrease += () => {
            heal += step;
        };
        bacteria.onPopulationDecrease += () => {
            heal -= step;
        };
    }

    public void OnDisable() {
        damageable = Option<IDamageable>.None();
    }

    public void IntervalHeal() {
        if (damageable.IsNone()) {
            damageable = new Option<IDamageable>(bacteria.ImmuneSystem.GetComponent<IDamageable>());
        }
        damageable.Get().Heal(heal);
    }
}
