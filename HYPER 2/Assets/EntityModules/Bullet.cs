using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stats.Value;

public class Bullet : MonoBehaviour
{
    private const float bulletLength = 120f;

    [SerializeField] private Stats stats;

    private Vector3 scale;
    private Quaternion lookAt;

    private bool DEAD_RIGHT_NOW = false;

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
        lookAt = Quaternion.LookRotation(stats.Manager.PlayerGo.transform.position-transform.position);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, lookAt.eulerAngles.z);

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
        collider.gameObject.GetComponent<IDamageable>()?.Damage(stats.get(DAMAGE));

        DEAD_RIGHT_NOW = true;
    }
}
