using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier : MonoBehaviour
{
    [SerializeField] private Bacteria bacteria;

    [SerializeField] private List<Stat> stats = new List<Stat>();
    [SerializeField] private List<float> incrementers = new List<float>();

    public void Start() {
        bacteria.onPopulationIncrease += () => {
            for (int i = 0; i < stats.Count; i++) {
                bacteria.Stats.inc(stats[i], incrementers[i]);
            }
        };
        bacteria.onPopulationDecrease += () => {
            if (bacteria.population <= 0) {
                for (int i = 0; i < stats.Count; i++) {
                bacteria.Stats.inc(stats[i], -incrementers[i]);
            }
            }
            for (int i = 0; i < stats.Count; i++) {
                bacteria.Stats.inc(stats[i], -incrementers[i]);
            }
        };
    }

    public void OnEnable() {
        for (int i = 0; i < stats.Count; i++) {
            bacteria.Stats.inc(stats[i], incrementers[i]);
        }
    }
}
