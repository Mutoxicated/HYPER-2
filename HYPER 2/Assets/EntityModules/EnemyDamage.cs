using UnityEngine;
using static Stat;

public class EnemyDamage : MonoBehaviour {
    [SerializeField] private Stats stats;
    private Option<IDamageable> currentlyColliding = Option<IDamageable>.None();

    public void IntervalDamage() {
        if (currentlyColliding.IsNone()) { return; }

        currentlyColliding.Get().Damage(stats, stats[DAMAGE]);
    }

    public void OnCollisionEnter(Collision collision) {
        //Debug.Log(collision.gameObject.name);
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null) {
            //Debug.Log("Contains Damageable");
            currentlyColliding = new Option<IDamageable>(damageable);
            IntervalDamage();
            stats.Die(new Option<Stats>(stats));
        }
    }

    // public void OnCollisionStay(Collision collision) {
    //     if (currentlyColliding.IsSome()) { return; }
        
    //     var damageable = collision.gameObject.GetComponent<IDamageable>();
    //     if (damageable != null) {
    //         currentlyColliding = new Option<IDamageable>(damageable);
    //     }
    // }

    public void OnCollisionExit() {
        currentlyColliding = Option<IDamageable>.None();
    }

    public void OnTriggerEnter(Collider collider) {
        //Debug.Log(collider.gameObject.name);
        var damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null) {
            //Debug.Log("Contains Damageable");
            currentlyColliding = new Option<IDamageable>(damageable);
            IntervalDamage();
            stats.Die(new Option<Stats>(stats));
        }
    }

    public void OnTriggerExit() {
        currentlyColliding = Option<IDamageable>.None();
    }
}