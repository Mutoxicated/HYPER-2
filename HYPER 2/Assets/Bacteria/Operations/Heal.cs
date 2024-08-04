using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] private Bacteria bacteria;
    [SerializeField] private float interval = 1f;
    [SerializeField] private float heal;
    [SerializeField] private float step;

    private Option<IDamageable> damageable = Option<IDamageable>.None();
    private float time = 0f;

    private void Start() {
        bacteria.onPopulationIncrease += () => {
            heal += step;
        };
        bacteria.onPopulationDecrease += () => {
            heal -= step;
        };
    }

    private void Update() {
        time += Time.deltaTime;
        if (time >= interval) {
            IntervalHeal();
            time = 0f;
        }
    }

    public void OnDisable() {
        damageable = Option<IDamageable>.None();
        time = 0f;
    }

    public void IntervalHeal() {
        if (damageable.IsNone()) {
            damageable = new Option<IDamageable>(bacteria.ImmuneSystem.GetComponent<IDamageable>());
        }
        damageable.Get().Heal(heal);
    }
}
