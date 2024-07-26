using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using static Stat;

[RequireComponent(typeof(Stats))]
public class Enemy : MonoBehaviour, ISideBelonger<Enemy>
{
    [SerializeField] private Stats stats;
    [SerializeField] private Resources.Enemy enemyType;
    [SerializeField] private HitVisual hitVisual;
    [SerializeField] private AudioSource hitSFX;
    [Header("Settings")]
    [SerializeField] private bool followPath;
    [SerializeField] private float XPWorth = 5f;
    
    private Branch currentBranch;

    private Vector3 dir;
    private float dist;
    private bool last;

    private World.ReportDeath<Enemy> death;

    private void PrepareConfiguration() {
        stats.ConfigureOnDie(s => {
            death.Invoke(this);
        });
    }

    public void CStart(Manager manager, World.ReportDeath<Enemy> death, Branch root) {
        // this.death = e => {
        //     manager.Pools.Get(PoolID.Explosion).ShootObject(e.transform.position, Quaternion.identity);
        // };
        this.death += death;
        stats.CStart(manager);
        PrepareConfiguration();

        transform.position += Vector3.up*(Resources.GetEnemyStats(enemyType).verticality-1)*2.5f;

        currentBranch = root;
        dir = Dir.ToVec(currentBranch.direction);
        look = Quaternion.LookRotation(currentBranch.end-currentBranch.start);
        gameObject.SetActive(true);
    }

    private Quaternion look;

    public void Update() {
        if (!followPath) { return; }
        //Debug.Log(dir);
        transform.position = Vector3.Lerp(transform.position, transform.position+dir*stats.get(SPEED), Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime*5f);
        //Debug.Log("Current branch: "+currentBranch.direction);
        if (last) {
            return;
        }
        if (currentBranch.direction == Direction.UP | currentBranch.direction == Direction.DOWN) {
            dist = Mathf.Abs(transform.position.z-currentBranch.end.z);
        }else {
            //Debug.Log(currentBranch.end);
            dist = Mathf.Abs(transform.position.x-currentBranch.end.x);
        }
        if (dist < 0.2f) {
            var branch = currentBranch.Next();
            //Debug.Log(branch);
            dist = 0f;
            if (branch.IsNone()) {
                last = true;
            }else {
                currentBranch = branch.Get();
                dir = Dir.ToVec(currentBranch.direction);
                look = Quaternion.LookRotation(currentBranch.end-currentBranch.start);
            }
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