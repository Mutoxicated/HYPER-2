using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HPBarOptions {
    public Transform hpbarAnchor;
    public float extrusion = 0f;
}


public class HPBarPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject player;

    private List<HPBar> actives = new List<HPBar>();
    private List<HPBar> inactives = new List<HPBar>();

    public void DStart(GameObject playerGo) {
        player = playerGo;
    }

    public void GetHPBar(HPBarOptions options, Stats stats) {
        GetHPBar(options.hpbarAnchor, options.extrusion, stats);
    }

    public void GetHPBar(Transform parent, float extrusion, Stats stats) {
        if (inactives.Count == 0) {
            var instance = Instantiate(prefab, transform).GetComponent<HPBar>();
            actives.Add(instance);
            instance.CStart(this, player);
            instance.Enable(parent, extrusion, stats);
        }else {
            var bar = inactives[0];
            inactives.Remove(bar);
            actives.Add(bar);
            bar.Enable(parent, extrusion, stats);
        }
    }

    public void RetrieveHPBar(HPBar hpBar) {
        actives.Remove(hpBar);
        inactives.Add(hpBar);
    }
}
