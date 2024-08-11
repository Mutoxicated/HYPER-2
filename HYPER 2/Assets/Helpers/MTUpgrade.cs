using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MTStatModifier
{
    public List<Stat> stats = new List<Stat>();
    public List<float> incrementers = new List<float>();
}

[CreateAssetMenu(fileName = "MTUpgrade", menuName = "Mutox/MTUpgrade")]
public class MTUpgrade : ScriptableObject
{
    [SerializeField] private bool stackable = true;
    [SerializeField] private Option<List<Element>> bacteria;
    [SerializeField] private Option<MTStatModifier> mtStatModifier = Option<MTStatModifier>.None();

    public void Action(MiniTower mt) {
        mtStatModifier.GetAndThen(sm => {
            for (int i = 0; i < sm.stats.Count; i++) {
                mt.Stats.inc(sm.stats[i], -sm.incrementers[i]);
            }
        });
        bacteria.GetAndThen(bc => {
            mt.Stats.Infector.GetAndThen(i => {
                i.Inherit(bc);
            });
        });
    }
}
