using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float health = 30f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float speed = 2f;

    [Header("Systems")]
    [SerializeField] private Interval damageInterval;
    
    private Branch currentBranch;

    private Vector3 dir;
    private float dist;
    private bool last;

    private Option<IDamageable> currentlyColliding = Option<IDamageable>.None();

    public void CStart(Branch root) {
        currentBranch = root;
        dir = transform.InverseTransformDirection(Dir.ToVec(currentBranch.direction));
        gameObject.SetActive(true);
    }

    public void Update() {
        transform.position = Vector3.Lerp(transform.position, transform.position+dir*speed, Time.deltaTime);
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

        currentlyColliding.Get().Damage(damage);
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

    public void Die() {
        Destroy(gameObject);
    }

    public void Damage(float amount)
    {
        if (health <= 0) { return; }
        health -= amount;
        if (health <= 0) {
            Die();
        }
    }
}