using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Events;

public class Bacteria : MonoBehaviour
{
    private const float rateStep = 0.05f;

    [SerializeField] private PooledObject pooledObject;
    [SerializeField] private Element @type;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float lifetimeIncrement = 0.05f;
    [SerializeField] private ParticleSystem ps;

    public int population = 1;

    public UnityAction onPopulationIncrease;
    public UnityAction onPopulationDecrease;

    private ImmuneSystem immune;
    public delegate void BacteriaDeath(Bacteria b);
    public float time = 0f;

    public ImmuneSystem ImmuneSystem {
        get { return immune; }
    }

    public Stats Stats {
        get { return ImmuneSystem.Stats; }
    }

    public Element Type {
        get { return @type; }
    }

    public void CStart(ImmuneSystem immune, BacteriaDeath onDeath) {
        onPopulationDecrease = new UnityAction(() => {
            if (population <= 0) {
                onDeath.Invoke(this);
                pooledObject.OnDeath();
                return;
            }
            population--;
            lifetime -= lifetimeIncrement;
            var emission = ps.emission;
            emission.rateOverTimeMultiplier -= rateStep;
        });
        onPopulationIncrease = new UnityAction(() => {
            population++;
            lifetime += lifetimeIncrement;
            var emission = ps.emission;
            emission.rateOverTimeMultiplier += rateStep;
        });
        this.immune = immune;
        immune.Stats.ConfigureOnBeforeDeath(false, (_) => {
            pooledObject.OnDeath();
            gameObject.SetActive(false);
        });
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
