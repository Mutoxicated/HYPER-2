using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotate : Rotate
{
    [SerializeField] private Stats stats;
    public override float Speed {
        get { return base.Speed*stats.get(Stat.SPEED); }
    } 
}
