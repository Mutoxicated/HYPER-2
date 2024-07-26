using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
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

    private void shootBullet() {
        var shot = false;
        for (int i = 0; i < shootAmount; i++) {
            if (!stats.TargetIsValid(i)) { continue; }

            var rotation = Quaternion.LookRotation(stats.Target(i).Get().transform.position-firePoint.position);
            shot = true;
            Action(firePoint.position, b => b.CStart(stats.Manager, rotation, stats));
        }
        if (shot) {
            shootSFX.Play();
        }
        enemies.Clear();
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
}