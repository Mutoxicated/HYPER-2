using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using static Stat;

public class Fly : MonoBehaviour
{
    [SerializeField] private Transform hypotheticalLook;
    [SerializeField] private Option<Transform> body;
    [SerializeField] private Stats stats;
    [SerializeField] private float accelRate;

    private float accel = 1f;
    private float delta;
    private Quaternion look;
    private GameObject target;

    private void Start() {
        Assert.IsTrue(stats.Range.IsSome());
    }

    private void Update() {
        stats.Range.GetAndThen(r => {
            r.Target().GetAndThen(s => {
                target = s.gameObject;
            }).Else(() => {
                target = stats.Manager.Tower;
            });

            look = Quaternion.LookRotation(target.transform.position+Vector3.up*4f-transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime*0.6f);

            hypotheticalLook.rotation = look;
            var vdelta = hypotheticalLook.forward-transform.forward;
            Vec3.Abs(ref vdelta);
            delta = (vdelta.x+vdelta.y+vdelta.z)/3f;
            delta -= 1;
            delta = Mathf.Abs(delta);
            if (delta > 0.55f) {
                accel += accelRate*stats.get2(RATE) * Time.deltaTime;
            }else {
               accel -= accelRate*stats.get2(RATE) * Time.deltaTime;
                if (accel < 1) {
                    accel = 1;
                }
            }
            //Debug.Log(delta);
            body.GetAndThen(b => {
                b.rotation = Quaternion.Lerp(b.rotation, b.rotation*Quaternion.AngleAxis(delta+accel, Vector3.forward), Time.deltaTime*2f);
            });
            //Debug.Log("C: "+uh);
            //Debug.Log(delta);
            transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats[SPEED]*accel, Time.deltaTime*delta);
        });
    }
}
