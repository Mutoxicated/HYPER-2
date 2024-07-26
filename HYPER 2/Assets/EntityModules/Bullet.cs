using System.Collections;
using System.Collections.Generic;
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

    private Vector3 scale;

    private bool DEAD_RIGHT_NOW = false;
    private float t;

    public void CStart(Manager manager, Quaternion direction, Stats other) {
        stats.CStart(manager, other);
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
            scale.z = Mathf.Lerp(scale.z, scale.z-stats.get(SPEED)*24f, Time.deltaTime);
            transform.localScale = scale;
            if (scale.z <= 0) {
                Destroy(gameObject);
            } 
            return;
        }

        transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats.get(SPEED), Time.deltaTime);
        scale = transform.localScale;
        if (scale.z > bulletLength) {
            scale.z = bulletLength;
        }else {
            scale.z = Mathf.Lerp(scale.z, scale.z+stats.get(SPEED)*19f, Time.deltaTime);
        }
        transform.localScale = scale;
    }

     public void OnTriggerEnter(Collider collider) {
        //Debug.Log("collider: "+collider.gameObject.name);
        collider.gameObject.GetComponent<IDamageable>()?.Damage(stats, stats.get(DAMAGE));

        var damageable = collider.gameObject.GetComponent<IDamageable>();
        int? xp = damageable?.Damage(stats, stats.get(DAMAGE));
        stats.Inheriter.GetAndThen(s => {
            if (s.StatSide == Stats.Side.PLAYER) {
                Player.IncrementXP(xp ?? 0);
                s.IncXP(xp ?? 0);
            }
        });

        stats.Die(Option<Stats>.None());
    }
}
