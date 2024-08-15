using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Helpers;
using Unity.VisualScripting;
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
    public bool stackable = true;
    public Option<List<Element>> bacteria;
    public Option<MTStatModifier> mtStatModifier = Option<MTStatModifier>.None();

    public void Action(MiniTower mt) {
        mtStatModifier.GetAndThen(sm => {
            for (int i = 0; i < sm.stats.Count; i++) {
                mt.Stats.inc(sm.stats[i], sm.incrementers[i]);
            }
        });
        bacteria.GetAndThen(bc => {
            mt.Stats.Infector.GetAndThen(i => {
                i.Inherit(bc);
            });
        });
    }

    public string Description() {
        var desc = new StringBuilder();

        var success = bacteria.GetAndThen(bs => {
            desc.Append("\n");
            desc.Append("Infect: ");

            for (int i = 0; i < bs.Count; i++) {
                desc.Append(Elm.ToString(bs[i]));
                if (i != bs.Count-1) {
                    desc.Append(", ");
                }
            }
        }).Successful();

        mtStatModifier.GetAndThen(sm => {
            desc.Append("\n");
            if (success) {
                desc.Append("\n");
            }

            for (int i = 0; i < sm.stats.Count; i++) {
                desc.AppendFormat("{0}:{1}{2}", sm.stats[i].ToString(), "<color=#00ff00>+", sm.incrementers[i]);
                if (i != sm.stats.Count-1) {
                    desc.Append("\n");
                } 
            }
        });

        return desc.ToString();
    }
}
