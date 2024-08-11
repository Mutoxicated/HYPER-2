using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infector : MonoBehaviour
{
    [SerializeField] private Stats stats;

    [SerializeField] private List<Element> bacteria = new List<Element>();

    public void Infect(GameObject gameObject) {
        var infectable = gameObject.GetComponent<IInfectable>();
        if (infectable == null) {
            return;
        }
        var counter = 0;
        for (int i = 0; i < bacteria.Count; i++) {
            if (Seed.Chance(stats[Stat.CHANCE])) {
                infectable.Infect(bacteria[i]);
                counter++;
                if (counter >= stats[Stat.AMOUNT]) {
                    return;
                }
            }
        }
    }

    public void Inherit(Infector other) {
        Inherit(other.bacteria);
    }

    public void Inherit(List<Element> other) {
        for (int i = 0; i < other.Count; i++) {
            bool containsBac = false;
            for (int j = 0; j < bacteria.Count; j++) {
                if (other[i] == bacteria[j]) {
                    containsBac = true;
                    break;
                }
            }
            if (!containsBac) {
                bacteria.Add(other[i]);
            }
        }
    }
}
