using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infector : MonoBehaviour// Provides the tools for other scripts to use in regards to infections
{
    [SerializeField] private Dictionary<Element, float> bacteriaChances = new Dictionary<Element, float>();
    [SerializeField] private int injectionsPerInfection = 1;

    public void Infect(GameObject gameObject) {
        var infectable = gameObject.GetComponent<IInfectable>();
        if (infectable == null) {
            return;
        }
        var counter = 0;
        foreach (var elm in bacteriaChances.Keys) {
            if (Seed.Chance(bacteriaChances[elm])) {
                infectable.Infect(elm);
                counter++;
                if (counter >= injectionsPerInfection) {
                    return;
                }
            }
        }
    }
}
