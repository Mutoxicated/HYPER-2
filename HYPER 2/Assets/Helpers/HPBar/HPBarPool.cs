using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject player;

    private List<HPBar> actives = new List<HPBar>();
    private List<HPBar> inactives = new List<HPBar>();

    public void DStart(GameObject playerGo) {
        player = playerGo;
    }

    public void GetHPBar(Transform parent, float extrusion, Stats stats) {
        GetHPBar(parent, extrusion, stats, Vector2.one);
    }

    public void GetHPBar(Transform parent, float extrusion, Stats stats, Vector2 scale) {
        if (inactives.Count == 0) {
            var instance = Instantiate(prefab, transform).GetComponent<HPBar>();
            actives.Add(instance);
            instance.CStart(this, player);
            instance.Enable(parent, extrusion, stats, scale);
        }else {
            var bar = inactives[0];
            inactives.Remove(bar);
            actives.Add(bar);
            bar.Enable(parent, extrusion, stats, scale);
        }
    }

    public void RetrieveHPBar(HPBar hpBar) {
        actives.Remove(hpBar);
        inactives.Add(hpBar);
    }
}
