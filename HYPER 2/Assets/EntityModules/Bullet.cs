using System.Collections;
using System.Collections.Generic;
using Helpers;
using Unity.VisualScripting;
using UnityEngine;
using static Stat;

public interface IBulletConstruct {
    void CStart(Manager manager, Quaternion direction, Stats other);
}

public class Bullet : MonoBehaviour, IBulletConstruct
{
    private const float bulletLength = 90f;
    private const float expiration = 5f;

    [SerializeField] private Stats stats;
    [SerializeField] private Resources.Bullet bulletType;

    private Vector3 scale;

    private bool DEAD_RIGHT_NOW = false;
    private float t;

    public void CStart(Manager manager, Quaternion direction, Stats other) {
        if (other.StatSide == Stats.Side.ENEMY) {
            Go.SetLayerRecursive(gameObject, EditorLayer.ENEMY_BULLET);
        }
        stats.CStart(manager, Resources.GetBulletStats(bulletType), other);
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void Update() {
        t += Time.deltaTime;
        if (t >= expiration) {
            DEAD_RIGHT_NOW = true;
        }

        if (DEAD_RIGHT_NOW) {
            scale = transform.localScale;
            scale.z = Mathf.Lerp(scale.z, scale.z-stats[SPEED]*24f, Time.deltaTime);
            transform.localScale = scale;
            if (scale.z <= 0) {
                Destroy(gameObject);
            } 
            return;
        }

        transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats[SPEED], Time.deltaTime);
        scale = transform.localScale;
        if (scale.z > bulletLength) {
            scale.z = bulletLength;
        }else {
            scale.z = Mathf.Lerp(scale.z, scale.z+stats[SPEED]*19f, Time.deltaTime);
        }
        transform.localScale = scale;
    }

    public void OnTriggerEnter(Collider collider) {
        //Debug.Log("collider: "+collider.gameObject.name);
        var damageable = collider.gameObject.GetComponent<IDamageable>();
        int? xp = damageable?.Damage(stats, stats[DAMAGE]);
        stats.Inheriter.GetAndThen(s => {
            if (s.StatSide == Stats.Side.PLAYER) {
                Player.IncrementXP(xp ?? 0);
                s.IncXP(xp ?? 0);
            }
        });

        stats.Die(Option<Stats>.None());
    }
}
