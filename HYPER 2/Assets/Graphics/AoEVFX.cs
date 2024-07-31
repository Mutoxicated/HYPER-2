using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class AoEVFX : MonoBehaviour
{
    [SerializeField] private Option<Stats> stats;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float lineWidth = 0.8f;
    [SerializeField] private float lineAmount = 10;
    [SerializeField] private float scale = 1f;
    [SerializeField] private float interval = 2f;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private float properScale {
        get { 
            if (stats.IsNone()) {
                return scale;
            }
            return stats.Get().Manager.World.WG.RangeSize(stats.Get()[Stat.RANGE]); 
        }
    }
    private float step {
        get { return 2*properScale/lineAmount; }
    }
    private float timeStep {
        get { return step/interval; }
    }
    private float accumulatedTimeSteps = 0f;

    public void CStart() {
        for (int i = 0; i < lineAmount; i++) {
            var instance = Instantiate(Resources.GetMisc(Resources.Misc.Line), transform);
            instance.transform.localPosition = Vector3.zero;
            instance.SetActive(true);
            var line = instance.GetComponent<LineRenderer>();
            line.startColor = color;
            line.endColor = color;
            lines.Add(line);
        }
        gameObject.SetActive(true);
    }

    private float offset;
    public void Update() {
        offset = properScale*0.5f;
        for (int i = 0; i < lineAmount; i++) {
            lines[i].startWidth = lineWidth/10f*properScale;
            lines[i].endWidth = lines[i].startWidth;
            var length = i*step+accumulatedTimeSteps;
            var extra = Num.MinLimit(length-properScale, 0);
            lines[i].SetPosition(0, new Vector3(length-extra-offset, 0f, extra-offset));
            lines[i].SetPosition(1, new Vector3(extra-offset, 0f, length-extra-offset));
        }
        accumulatedTimeSteps += timeStep * Time.deltaTime;
        if (accumulatedTimeSteps >= step) {
            accumulatedTimeSteps = 0f;
        }
    }
}
