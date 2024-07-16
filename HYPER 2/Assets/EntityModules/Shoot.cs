using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private Resources.Bullet bullet;
    [SerializeField] private Stats stats;
    [SerializeField] private int shootAmount = 1;
    [SerializeField] private bool inherit = false;
    [SerializeField] private Interval shootInterval;
    [SerializeField] private Transform firePoint;

    public delegate void BulletConstructor(Bullet bullet);

    private List<GameObject> enemies = new List<GameObject>();

    private void OnEnable() {
        shootInterval.AddEvent(shootBullet);
    }

    private void shootBullet() {
        for (int i = 0; i < shootAmount; i++) {
            if (!stats.TargetIsValid(i)) { return; }

            var rotation = Quaternion.LookRotation(stats.Target(i).Get().transform.position-firePoint.position);

            if (inherit) 
                Action(firePoint.position, b => b.CStart(stats.Manager, rotation, stats));
            else
                Action(firePoint.position, b => b.CStart(stats.Manager, rotation));
        }
        enemies.Clear();
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