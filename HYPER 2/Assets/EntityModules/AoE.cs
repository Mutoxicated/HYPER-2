using AYellowpaper;
using UnityEngine;
using static Stat;

public class AoE : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Option<AoEVFX> vfx;

    private float time = 0f;
    private Range range;

    private void Start() {
        stats.Range.TryGetOr(() => { 
                Debug.LogError("AoE Component requires stats to have a range to work"); 
                enabled = false; 
                return; 
            }
        );
        stats.Infector.TryGetOr(() => { 
                Debug.LogError("AoE Component requires stats to have an infector to work"); 
                enabled = false; 
                return; 
            }
        );
        range = stats.Range.Get();
        vfx.GetAndThen(v => v.CStart());
        range.OnEnter += e => {
            stats.Infector.Get().Infect(e.gameObject);
        };
    }

    private void IntervalInfect() {
        range.Foreach(s => {
            stats.Infector.Get().Infect(s.gameObject);
        });
    }

    private void Update() {
        time += Time.deltaTime;
        if (time >= stats.get2(RATE)) {
            time = 0f;
            IntervalInfect();
        }
    }
}
