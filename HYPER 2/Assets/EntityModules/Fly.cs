using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using static Stat;

public class Fly : MonoBehaviour
{
    [SerializeField] private Stats stats;

    private float delta;
    private Quaternion look;
    private GameObject target;

    private void Start() {
        Assert.IsTrue(stats.Range.IsSome());
    }

    private void Update() {
        stats.Range.GetAndThen(r => {
            bool got = r.Value.Find(g => {
                return g.gameObject.layer == EditorState.GetLayer(EditorLayer.DEFAULT);
            }).GetAndThen(g => {
                target = g;
            }).Successful();

            if (!got) {
                target = stats.Manager.Tower;
            }

            look = Quaternion.LookRotation(target.transform.position+Vector3.up*5f-transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime*0.5f);
            Debug.Log("1:" +transform.eulerAngles);
            Debug.Log("2:" +look.eulerAngles);
            var uh = new Vector3(
                Num.SafeDiv(transform.eulerAngles.x, look.eulerAngles.x),
                Num.SafeDiv(transform.eulerAngles.y, look.eulerAngles.y),
                Num.SafeDiv(transform.eulerAngles.z, look.eulerAngles.z)
            );
            Debug.Log("C: "+uh);
            Vec3.Abs(ref uh);
            delta = (uh.x+uh.y+uh.z)/3f;
            Debug.Log(delta);
            transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*stats.get(SPEED), Time.deltaTime*delta);
        });
    }
}
