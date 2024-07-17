using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using static Stat;

public class Enemy : MonoBehaviour, IDamageable, ISideBelonger<Enemy>
{
    [SerializeField] private Stats stats;
    [Header("Settings")]
    [SerializeField] private bool followPath;
    [SerializeField] private float XPWorth = 5f;
    private health hp;
    
    private Branch currentBranch;

    private Vector3 dir;
    private float dist;
    private bool last;

    private World.ReportDeath<Enemy> death;

    public void CStart(Manager manager, World.ReportDeath<Enemy> death, Branch root) {
        this.death = death;
        hp = stats.CStart(manager);

        currentBranch = root;
        dir = transform.InverseTransformDirection(Dir.ToVec(currentBranch.direction));
        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, World.ReportDeath<Enemy> death) {
        this.death = death;
        hp = stats.CStart(manager);

        gameObject.SetActive(true);
    }

    public void Update() {
        if (!followPath) { return; }
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

    public void Die(Option<Stats> senderStats) {
        if (senderStats.IsNone()) {
            death.Invoke(this);
            Destroy(gameObject);
            return;
        }
        if (senderStats.Get().StatSide == Stats.Side.PLAYER) {
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
            Die(new Option<Stats>(stats));
        }
    }

    public void BacteriaDamage(Element bt, float amount) {
        if (hp.Current <= 0) { return; }

        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }

    public Stats.Side Side()
    {
        return stats.StatSide;
    }

    public GameObject Go()
    {
        return gameObject;
    }

    public bool IsNull() {
        return this == null;
    }

    public ref World.ReportDeath<Enemy> GetReportDeath() {
        return ref death;
    }
}