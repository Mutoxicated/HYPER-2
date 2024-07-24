
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public enum Strength {
        WEAK = 5,
        MEDIUM = 10,
        STRONG = 20,
        POWERFUL = 40
    }

    private const float expiration = 1.2f;

    [SerializeField] private Strength strength;
    [SerializeField] private Option<Infector> infector;
    [SerializeField] private AnimationCurve scaleModifier;
    [SerializeField] private MaterialChanger mc;

    private float time = 0f;
    private float scaleT = 1f;
    private float t = 0f;
    private Vector3 expandedScale;
    private List<GameObject> explodedGos = new List<GameObject>();
    private Color initColor;
    private Material material;

    public void CStart() {
        expandedScale = transform.localScale;
        transform.localScale = Vector3.zero;

        var keys = scaleModifier.keys;
        for (int i = 0; i < keys.Length; i++) {
            keys[i].time *= expiration;
        }
        scaleModifier.keys = keys;
        scaleModifier.postWrapMode = WrapMode.ClampForever;

        initColor = mc.Color;
        mc.CStart();
        material = mc.material;
    }

    public void Action() {
        transform.parent = null;
        gameObject.SetActive(true);
    }

    private void Update() {
        time += Time.deltaTime;

        scaleT = scaleModifier.Evaluate(time);
        transform.localScale = expandedScale*scaleT;

        t = Mathf.Abs((time / expiration)-1);
        mc.Color.a = initColor.a*t;

        if (time >= expiration/2f) {
            material.SetFloat("_Intact", Mathf.Lerp(expiration/2f, expiration, time));
        }
        
        if (time >= expiration) {
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter(Collider col) {
        foreach (var exploded in explodedGos) {
            if (exploded == col.gameObject) {
                return;
            }
        }
        explodedGos.Add(col.gameObject);
        infector.GetAndThen(i => {
            i.Infect(col.gameObject);
        }).Else(() => {
            col.gameObject.GetComponent<IDamageable>()?.ExplosionDamage(strength, 0);
        });
    }
}