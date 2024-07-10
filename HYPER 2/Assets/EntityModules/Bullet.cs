using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stat;

public class Bullet : MonoBehaviour
{
    private const float bulletLength = 90f;
    private const float expiration = 5f;

    [SerializeField] private Stats stats;

    private Vector3 scale;
    private Quaternion lookAt;

    private bool DEAD_RIGHT_NOW = false;
    private float t;

    public void CStart(Manager manager, Quaternion direction, Stats _) {
        stats.CStart(manager);
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, Quaternion direction) {
        stats.CStart(manager);
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void Update() {
        t += Time.deltaTime;
        if (t >= 5f) {
            DEAD_RIGHT_NOW = true;
        }

        if (DEAD_RIGHT_NOW) {
            scale = transform.localScale;
            scale.z = Mathf.Lerp(scale.z, scale.z-stats.get(SPEED)*24f, Time.deltaTime);
            transform.localScale = scale;
            if (scale.z <= 0) {
                Destroy(gameObject);
            } 
            return;
        }

        transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats.get(SPEED), Time.deltaTime);
        scale = transform.localScale;
        if (scale.z > bulletLength) {
            scale.z = bulletLength;
        }else {
            scale.z = Mathf.Lerp(scale.z, scale.z+stats.get(SPEED)*19f, Time.deltaTime);
        }
        transform.localScale = scale;
    }

    public void OnTriggerEnter(Collider collider) {
        //Debug.Log("collider: "+collider.gameObject.name);
        collider.gameObject.GetComponent<IDamageable>()?.Damage(stats, stats.get(DAMAGE));

        DEAD_RIGHT_NOW = true;
    }
}
