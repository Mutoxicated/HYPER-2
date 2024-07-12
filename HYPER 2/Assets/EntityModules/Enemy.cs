using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using static Stat;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Stats stats;
    [Header("Systems")]
    [SerializeField] private Interval damageInterval;
    [Header("Settings")]
    [SerializeField] private float XPWorth = 5f;
    private health hp;
    
    private Branch currentBranch;

    private Vector3 dir;
    private float dist;
    private bool last;

    private Option<IDamageable> currentlyColliding = Option<IDamageable>.None();
    private World.ReportDeath death;

    public void CStart(Manager manager, World.ReportDeath death, Branch root) {
        this.death = death;
        hp = stats.CStart(manager);

        currentBranch = root;
        dir = transform.InverseTransformDirection(Dir.ToVec(currentBranch.direction));
        gameObject.SetActive(true);
    }

    public void ReportDeath(World.ReportDeath death) {
        this.death += death;
    }

    public void Update() {
        transform.position = Vector3.Lerp(transform.position, transform.position+dir*stats.get(SPEED), Time.deltaTime);
        //Debug.Log("Current branch: "+currentBranch.direction);
        if (last) {
            return;
        }
        if (currentBranch.direction == Direction.UP | currentBranch.direction == Direction.DOWN) {
            dist = Mathf.Abs(transform.position.z-currentBranch.end.transform.position.z);
        }else {
            //Debug.Log(currentBranch.end);
            dist = Mathf.Abs(transform.position.x-currentBranch.end.transform.position.x);
        }
        if (dist < 0.2f) {
            var branch = currentBranch.Next();
            //Debug.Log(branch);
            dist = 0f;
            if (branch.IsNone()) {
                last = true;
            }else {
                currentBranch = branch.Get();
                dir = transform.InverseTransformDirection(Dir.ToVec(currentBranch.direction));
            }
        }
    }

    public void IntervalDamage() {
        if (currentlyColliding.IsNone()) { return; }

        currentlyColliding.Get().Damage(stats, stats.get(DAMAGE));
    }

    public void ReceiveBacteria() {

    }

    public void OnCollisionEnter(Collision collision) {
        //Debug.Log(collision.gameObject.name);
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null) {
            //Debug.Log("Contains Damageable");
            currentlyColliding = new Option<IDamageable>(damageable);
            IntervalDamage();
            damageInterval.Enable();
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
        damageInterval.Disable();
        currentlyColliding = Option<IDamageable>.None();
    }

    public void Die(Stats senderStats) {
        if (senderStats.StatSide == Stats.Side.PLAYER) {
            Player.IncrementXP(XPWorth);
        }
        death.Invoke(this);
        Destroy(gameObject);
    }

    public void Damage(Stats stats, float amount)
    {
        //Debug.Log("["+hp.Current+"] Being damaged: "+amount);
        //Debug.Log("from "+stats.gameObject.name);
        if (hp.Current <= 0) { return; }

        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(stats);
        }
    }
}