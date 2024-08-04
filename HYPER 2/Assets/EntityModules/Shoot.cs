using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyContainer {
    List<GameObject> Enemies();
}

public class Shoot : MonoBehaviour, IEnemyContainer
{
    [SerializeField] private Resources.Bullet bullet;
    [SerializeField] private Stats stats;
    [SerializeField] private int shootAmount = 1;
    [SerializeField] private Interval shootInterval;
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioSource shootSFX;

    public delegate void BulletConstructor(IBulletConstruct bullet);

    private List<GameObject> enemies = new List<GameObject>();

    private void OnEnable() {
        shootInterval.AddEvent(shootBullet);
    }

    public void shootBullet() {
        var targets = new GameObject[shootAmount];
        int filled = 0;
        int offset = 0;
        while (filled < shootAmount) {
            var msg = stats.TargetIsValid(offset);
            if (msg == TargetMessage.VALID) {
                targets[filled] = stats.Target(offset).Get().gameObject;
                filled++;
            }else if (msg == TargetMessage.OUT_OF_BOUNDS) {
                break;
            }
            offset++;
        }

        var shot = false;
        for (int i = 0; i < targets.Length; i++) {
            if (targets[i] == null) { continue; }
            var rotation = Quaternion.LookRotation(targets[i].transform.position-firePoint.position);
            shot = true;
            Action(firePoint.position, b => b.CStart(stats.Manager, rotation, stats));
        }
        if (shot) {
            shootSFX.Play();
        }
    }
    
    private GameObject Action(Vector3 postion, BulletConstructor bc) {
        var instance = Instantiate(Resources.GetBullet(bullet));
        var b = instance.GetComponent<IBulletConstruct>();
        instance.transform.position = postion;
        
        bc(b);
        return instance;
    }

    private Bullet Action(BulletConstructor bc) {
        var instance = Instantiate(Resources.GetBullet(bullet));
        var b = instance.GetComponent<Bullet>();
        
        bc(b);
        return b;
    }

    public List<GameObject> Enemies()
    {
        return enemies;
    }
}