using UnityEngine;
using Helpers;
using static Stat;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Resources.Enemy enemyType;
    [SerializeField] private AudioSource hitSFX;
    [Header("Settings")]
    [SerializeField] private Option<HPBarOptions> options;
    [SerializeField] private bool followPath;
    
    private Branch currentBranch;
    private Direction main;

    private Vector3 dir;
    private float dist;
    private bool last;

    private World.ReportDeath<Enemy> death;

    private void PrepareConfiguration() {
        stats.ConfigureOnDie(false, s => {
            // s.GetAndThen(sts => {
            //     if (sts.StatSide == Stats.Side.PLAYER) {
            //         Player.SetMoneyRelative(1);
            //     }
            // });
            death.Invoke(this);
        });
    }

    public void CStart(Manager manager, World.ReportDeath<Enemy> death, Branch root, Direction main) {
        // this.death = e => {
        //     manager.Pools.Get(PoolID.Explosion).ShootObject(e.transform.position, Quaternion.identity);
        // };
        this.main = main;
        this.death += death;
        stats.CStart(manager, Resources.GetEnemyStatTemplates(enemyType));
        stats.ConfigureOnLvlUp(true, () => {});
        PrepareConfiguration();
        for (int i = 1; i < Gameplay.EnemyLvl; i++) {
            stats.IncXP(stats.MaxRequiredXP());
        }
        transform.position += Vector3.up*(Resources.GetEnemyStats(enemyType).verticality-1)*3f;
        options.GetAndThen(a => {
            stats.Manager.GUI.HPBP.GetHPBar(a.hpbarAnchor, a.extrusion, stats);
        });

        currentBranch = root;
        dir = Dir.ToVec(currentBranch.direction);
        look = Quaternion.LookRotation(currentBranch.end.transform.position-currentBranch.start.transform.position);
        gameObject.SetActive(true);
    }

    private Quaternion look;

    public void Update() {
        if (!followPath) { return; }
        //Debug.Log(dir);
        transform.position = Vector3.Lerp(transform.position, transform.position+dir*stats[SPEED], Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime*5f);
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
                dir = Dir.ToVec(main);
                var direction = (stats.Manager.Tower.transform.position-transform.position).normalized;
                direction.y = 0f;
                Vec3.Round(direction);
                look = Quaternion.LookRotation(direction);
                last = true;
            }else {
                currentBranch = branch.Get();
                dir = Dir.ToVec(currentBranch.direction);
                look = Quaternion.LookRotation(currentBranch.end.transform.position-currentBranch.start.transform.position);
            }
        }
    }
}