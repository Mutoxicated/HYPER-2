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
    private Dictionary<Element, Bacteria> bacterias = new  Dictionary<Element, Bacteria>();

    public void Infect(Element bt) {
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
            Pool.FromBT(stats.Manager, b.Type)
                .Get()
                .ReattachImmediate(b.gameObject);
        });
 
        bacterias.Add(bac.Type, bac);
    }
}
