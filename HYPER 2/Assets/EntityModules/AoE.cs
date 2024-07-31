using AYellowpaper;
using UnityEngine;
using static Stat;

public class AoE : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Element bt;

    private Range range;

    private void Start() {
        stats.Range.TryGetOr(() => { 
            Debug.LogError("AoE Component requires stats to have a range to work"); 
            enabled = false; 
            return; 
            }
        );
        range = stats.Range.Get();
        //range.OnEnter += ApplyEffect;
    }

    public void IntervalDamage() {
        range.Foreach(s => {
            s.gameObject.GetComponent<IInfectable>()?.Infect(bt);
        });
    }

    // private void OnDestroy(){
    //     range.OnEnter -= ApplyEffect;
    // }

    // public void ApplyEffect(Enemy e) {
        
    // }
}
