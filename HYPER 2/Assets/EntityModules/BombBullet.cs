using UnityEngine;
using static Stat;

public class BombBullet : MonoBehaviour, IBulletConstruct {
    private const float expiration = 5f;

    [SerializeField] private Stats stats;
    [SerializeField] private Explosion explosion;

    private float t;

    public void CStart(Manager manager, Quaternion direction, Stats other) {
        stats.CStart(manager);
        explosion.CStart();
        stats.Inherit(other);
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, Quaternion direction) {
        stats.CStart(manager);
        explosion.CStart();
        transform.rotation = direction;
        gameObject.SetActive(true);
    }
 
    private void Die() {
        explosion.Action();
        Destroy(gameObject);
    }

    public void Update() {
        t += Time.deltaTime;
        if (t >= expiration) {
            Die();
            return;
        }

        transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats.get(SPEED), Time.deltaTime);
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

        Die();
    }
}