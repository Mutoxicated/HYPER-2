using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stats.Normalized;

public class Bullet : MonoBehaviour
{
    private const float bulletLength = 120f;

    [SerializeField] private Stats stats;

    [SerializeField, Range(3f, 20f)] private float speed = 10f;

    private Vector3 scale;

    public void CStart(Quaternion direction, Stats _) {
        stats.CStart();
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void CStart(Quaternion direction) {
        stats.CStart();
        transform.rotation = direction;
        gameObject.SetActive(true);
    }

    public void Update() {
        transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*speed*stats.get[SPEED], Time.deltaTime);
        scale = transform.localScale;
        if (scale.z > bulletLength) {
            scale.z = bulletLength;
        }else {
            scale.z += speed*stats.get[SPEED];
        }
        transform.localScale = scale;
    }
}
