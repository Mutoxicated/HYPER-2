using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunData
{
    //MACRO_START
    [SerializeField] private int __seed = -1;
    
    public int Seed {
    	get { return __seed; }
    }
    
    public void SetSeed(int val) {
    	__seed = val;
    }

    [SerializeField] private int __money = 10;
    
    public int Money {
    	get { return __money; }
    }
    
    public void SetMoney(int val) {
    	__money = val;
    }
    [SerializeField] private float __xp = 0;
    
    public float XP {
    	get { return __xp; }
    }
    
    public void SetXP(float val) {
    	__xp = val;
    }

    [SerializeField] private int __lvl = 1;
    
    public int Lvl {
    	get { return __lvl; }
    }
    
    public void SetLvl(int val) {
    	__lvl = val;
    }
    //MACRO_END
    [SerializeField] private List<Resources.Arsenal> enabledArsenals = new List<Resources.Arsenal>() {
        Resources.Arsenal.FIRST_MT
    };
}
