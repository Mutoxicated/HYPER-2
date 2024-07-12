using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private Resources.Bullet bullet;
    [SerializeField] private Stats stats;
    [SerializeField] private Interval shootInterval;
    [SerializeField] private Transform firePoint;

    public delegate void BulletConstructor(Bullet bullet);

    private void OnEnable() {
        shootInterval.AddEvent(shootBullet);
    }

    private void shootBullet() {
        if (stats.TargetIsValid())
            Action(firePoint.position, b => b.CStart(stats.Manager, firePoint.rotation));
    }
    
    public Bullet Action(Vector3 postion, BulletConstructor bc) {
        var instance = Instantiate(Resources.GetBullet(bullet));
        var b = instance.GetComponent<Bullet>();
        instance.transform.position = postion;
        
        bc(b);
        return b;
    }

    public Bullet Action(BulletConstructor bc) {
        var instance = Instantiate(Resources.GetBullet(bullet));
        var b = instance.GetComponent<Bullet>();
        
        bc(b);
        return b;
    }
}