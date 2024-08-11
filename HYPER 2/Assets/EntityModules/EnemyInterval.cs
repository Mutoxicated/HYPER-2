using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterval : Interval
{
    [SerializeField] private Stats stats;
    public override float Rate => base.Rate*stats.get2(Stat.RATE);
}
