using AYellowpaper;
using UnityEngine;
using static Stat;

public class AoE : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Element bt;
    [SerializeField] private Option<AoEVFX> vfx;

    private Range range;

    private void Start() {
        stats.Range.TryGetOr(() => { 
                Debug.LogError("AoE Component requires stats to have a range to work"); 
                enabled = false; 
                return; 
            }
        );
        range = stats.Range.Get();
        vfx.GetAndThen(v => v.CStart());
        range.OnEnter += Infect;
        //range.OnEnter += ApplyEffect;
    }

    public void Infect(Stats s) {
        if (range.Filter(s).IsSome())
            s.gameObject.GetComponent<IInfectable>()?.Infect(bt);
    }
}
