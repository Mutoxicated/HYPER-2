using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class Tower : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Stats stats;
    [SerializeField] private StatsTemplate template;
    [SerializeField] private DisplayTowerStatus dts;

    private void Awake() {
        stats.CStart(player.Manager, template);
        stats.ConfigureOnDie(true, _ => {
            Die(_);
        });
        dts.CStart();
    }

    private void OnCollisionEnter(Collision collision) {
        Destroy(collision.gameObject);
    }

    public void Die(Option<Stats> _) {
        player.DeadMode();
    }
}
