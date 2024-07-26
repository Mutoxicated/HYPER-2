using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class Tower : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Stats stats;

    private void OnCollisionEnter(Collision collision) {
        Destroy(collision.gameObject);
    }

    public void Die(Option<Stats> _) {
        player.DeadMode();
    }
}
