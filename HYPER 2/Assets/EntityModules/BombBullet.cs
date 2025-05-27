using Helpers;
using UnityEngine;
using static Stat;

public class BombBullet : MonoBehaviour, IBulletConstruct {
    private const float expiration = 5f;

    [SerializeField] private Resources.Bullet bulletType;
    [SerializeField] private Stats stats;
    [SerializeField] private Explosion explosion;

    private float t;

    public void CStart(Manager manager, Quaternion direction, Stats other) {
        if (other.StatSide == Stats.Side.ENEMY) {
            Go.SetLayerRecursive(gameObject, EditorLayer.ENEMY_BULLET);
        }
        stats.CStart(manager, Resources.GetBulletStats(bulletType));
        explosion.CStart();
        stats.Inherit(other);
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, Quaternion direction) {
        stats.CStart(manager, Resources.GetBulletStats(bulletType));
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

        transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats[SPEED], Time.deltaTime);
    }

    public void OnTriggerEnter(Collider collider) {
        //Debug.Log("collider: "+collider.gameObject.name);
        var damageable = collider.gameObject.GetComponent<IDamageable>();
        int? xp = damageable?.Damage(stats, stats[DAMAGE]);
        stats.Inheriter.GetAndThen(s => {
            if (s.StatSide == Stats.Side.PLAYER) {
                Player.self.IncrementXP(xp ?? 0);
                s.IncXP(xp ?? 0);
            }
        });

        Die();
    }
}