using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public interface IInfectable {
    void Infect(Element bt);
}

public class ImmuneSystem : MonoBehaviour, IInfectable
{
    [SerializeField] private Stats stats;
    public Stats Stats {
        get { return stats; }
    }
    private Dictionary<Element, Bacteria> bacterias = new  Dictionary<Element, Bacteria>();

    public void Infect(Element bt) {
        Debug.Log("called");
        if (stats.StatElmImmunities.Contains(bt)) {
            return;
        }
        if (bacterias.ContainsKey(bt)) {
            bacterias[bt].onPopulationIncrease.Invoke();
            return;
        }
        var bac = Pool.FromBT(stats.Manager, bt)
            .Get()
            .SendObject(gameObject)
            .GetComponent<Bacteria>();
        bac.CStart(this, b => { 
            bacterias.Remove(b.Type);
        });
 
        bacterias.Add(bac.Type, bac);
    }
}
