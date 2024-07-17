using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Assertions;

public class Fly : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private float speed;


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
            });

            if (!got) {
                target = stats.Manager.Tower;
            }

            look = Quaternion.LookRotation(target.transform.position+Vector3.up*5f-transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime*0.5f);
            var uh = new Vector3(
                look.eulerAngles.x/transform.eulerAngles.x,
                look.eulerAngles.y/transform.eulerAngles.y,
                look.eulerAngles.z/transform.eulerAngles.z
            );
            Vec3.Abs(ref uh);
            delta = (uh.x+uh.y+uh.z)/3f;
            Debug.Log(delta);
            transform.position = Vector3.Lerp(transform.position, transform.position+transform.forward*speed, Time.deltaTime*delta);
        });
    }
}
