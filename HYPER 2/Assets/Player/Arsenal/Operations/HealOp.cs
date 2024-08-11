using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOp : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Option<AoEVFX> vfx;
    
    private float time;

    private void Start() {
         stats.Range.TryGetOr(() => { 
                Debug.LogError("AoE Component requires stats to have a range to work"); 
                enabled = false; 
                return; 
            }
        );
        vfx.GetAndThen(v => v.CStart());
    }

    private void Update() {
        time += Time.deltaTime;

        if (time >= stats.get2(Stat.RATE)) {
            time = 0f;
            stats.Range.Get().Foreach(s => {
                s.Heal(stats[Stat.DAMAGE], true);
            });
        }
    }
}
