using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public interface IInfectable {
    void Infect(BacteriaType bt);
}

public class ImmuneSystem : MonoBehaviour, IInfectable
{
    [SerializeField] private Stats stats;
    private Dictionary<BacteriaType, Bacteria> bacterias = new  Dictionary<BacteriaType, Bacteria>();

    public void Infect(BacteriaType bt) {
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
