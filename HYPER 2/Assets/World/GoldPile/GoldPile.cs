using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPile : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private StatsTemplate template;

    private float yOffset = 4f;
    private Manager manager;

    public int OnHit() {
        manager.GUI.PopupPool.Popup(transform.position+Vector3.up*yOffset, Color.yellow, "+1");
        Player.SetMoneyRelative(1);
        return 0;
    }

    public void CStart(Manager manager) {
        stats.CStart(manager, template);
        this.manager = manager;
        stats.ConfigureOnStatDamage(false, (_, _) => {
            return OnHit();
        }); 
        stats.ConfigureOnBacteriaDamage(true, (_, _) => {
            return 0;
        }); 
        stats.ConfigureOnExplosionDamage(false, (_, _) => {
            return OnHit();
        }); 
    }
}
