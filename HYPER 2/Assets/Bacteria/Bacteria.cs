using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Events;

public enum BacteriaType {
    Water,
    Fire
}

public class Bacteria : MonoBehaviour
{
    private const float rateStep = 0.05f;

    [SerializeField] private BacteriaType bt;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private ParticleSystem ps;

    public int population = 1;

    public UnityAction onPopulationIncrease;
    public UnityAction onPopulationDecrease;

    private ImmuneSystem immune;
    public delegate void BacteriaDeath(Bacteria b);
    private float time = 0f;

    public ImmuneSystem ImmuneSystem {
        get { return immune; }
    }

    public BacteriaType Type {
        get { return bt; }
    }

    public void CStart(ImmuneSystem immune, BacteriaDeath onDeath) {
        onPopulationDecrease = new UnityAction(() => {
            population--;
            var emission = ps.emission;
            emission.rateOverTimeMultiplier -= rateStep;
            if (population <= 0) {
                onDeath.Invoke(this);
            }
        });
        onPopulationIncrease = new UnityAction(() => {
            population++;
            var emission = ps.emission;
            emission.rateOverTimeMultiplier += rateStep;
        });
        this.immune = immune;
        gameObject.SetActive(true);
    }

    private void Update() {
        time += Time.deltaTime;
        if (time > lifetime) {
            time = 0;
            onPopulationDecrease.Invoke();
        }
    }
}
