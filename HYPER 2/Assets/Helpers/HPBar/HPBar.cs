using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Transform inner;
    [SerializeField] private Image bar;

    private Stats stats;
    private Quaternion lookTo;

    private bool open = true;
    private bool close = false;
    private const float closeTime = 0.5f;
    private float time;

    private Vector3 initScale;
    private Vector3 maxScale;
    private Vector3 minScale;

    private HPBarPool hpbp;
    private GameObject playerGo;
    private Transform trans;

    public void CStart(HPBarPool hpbp, GameObject player) {
        this.hpbp = hpbp;
        initScale = inner.localScale;
        minScale = initScale;
        minScale.x = 0f;
        playerGo = player;
    }

    public void Enable(Transform trans, float extrusion, Stats stats, Vector2 scale) {
        this.stats = stats;
        maxScale = initScale;
        maxScale.x *= scale.x;
        maxScale.y *= scale.y;
        inner.localPosition = Vector3.forward*extrusion;
        this.trans = trans;
        gameObject.SetActive(true);
        open = true;
        var playerPos = playerGo.transform.position; playerPos.y = 0f;
        var myPos = transform.position; myPos.y = 0f;
        lookTo = Quaternion.LookRotation(playerPos-myPos);
        transform.rotation = lookTo;
        stats.ConfigureOnBeforeDeath(false, _ => {
            close = true;
        });
    }

    private void RootRotation() {
        var playerPos = playerGo.transform.position; playerPos.y = 0f;
        var myPos = transform.position; myPos.y = 0f;
        lookTo = Quaternion.LookRotation(playerPos-myPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookTo, Time.deltaTime*2f);
    }

    private void Update() {
        if (open) {
            time += Time.deltaTime;

            inner.localScale = Vector3.Lerp(minScale, maxScale, time/closeTime);

            if (inner.localScale == maxScale) {
                time = 0f;
                open = false;
            }
        }
        transform.position = trans.position;

        RootRotation();

        lookTo = Quaternion.LookRotation(playerGo.transform.position-inner.position);
        inner.rotation = Quaternion.Lerp(inner.rotation, lookTo, Time.deltaTime);

        bar.fillAmount = Mathf.Round(stats.HP)/Mathf.Round(stats[Stat.MAX_HEALTH]);
        if (close) {
            if (open)
                open = false;
            time += Time.deltaTime;

            inner.localScale = Vector3.Lerp(maxScale, minScale, time/closeTime);

            if (inner.localScale == minScale) {
                time = 0f;
                close = false;
                gameObject.SetActive(false);
                hpbp.RetrieveHPBar(this);
            }
        }
    }
}
