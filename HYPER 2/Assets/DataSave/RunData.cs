using System;
using System.Collections.Generic;
using UnityEngine;
using static Resources.Arsenal;

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

    public void SetMoneyRelative(int val) {
        __money += val;
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
    [SerializeField] private int __waves = 0;

    public int Waves {
        get { return __waves; }
    }

    public void SetWaves(int waves) {
        __waves = waves;
    }

    public void IncrementWaves() {
        __waves++;
    }

    [SerializeField] private List<Resources.Arsenal> boughtArsenals = new List<Resources.Arsenal>() {
        FIRST_MT
    };

    public void AddBoughtArsenal(Resources.Arsenal arsenal) {
        boughtArsenals.Add(arsenal);
    }

    public void RemoveBoughtArsenal(Resources.Arsenal arsenal) {
        boughtArsenals.Remove(arsenal);
    }

    public List<Resources.Arsenal> BoughtArsenals {
        get { return boughtArsenals; }
    }

    [SerializeField] private List<Option<Resources.Arsenal>> quickInventoryArsenals = new List<Option<Resources.Arsenal>>() {
        new Option<Resources.Arsenal>(FIRST_MT)
    };

    public void UpdateQIACapacity(int capacity) {
        quickInventoryArsenals.Capacity = capacity;
        for (int i = 0; i < capacity; i++) {
            if (quickInventoryArsenals.Count < i) {
                quickInventoryArsenals.Add(Option<Resources.Arsenal>.None());
            }
        }
    }

    public void UpdateQIAIndex(int index, Resources.Arsenal arsenal) {
        quickInventoryArsenals[index] = new Option<Resources.Arsenal>(arsenal);
    }

    public List<Option<Resources.Arsenal>> QIAs {
        get { return quickInventoryArsenals; }
    }

    [SerializeField] private int selectedIndex = 0;

    public int SelectedIndex {
        get { return selectedIndex; }
    }

    public void SetSelectedIndex(int index) {
        selectedIndex = index;
    }

    // [SerializeField] private List<Resources.Arsenal> researchedArsenals = new List<Resources.Arsenal>() {
    //     FIRST_MT
    // };

    // public void AddResearchedArsenal(Resources.Arsenal arsenal) {
    //     boughtArsenals.Add(arsenal);
    // }

    // public void RemoveResearchedArsenal(Resources.Arsenal arsenal) {
    //     boughtArsenals.Remove(arsenal);
    // }

    // public List<Resources.Arsenal> ResearchedArsenals {
    //     get { return researchedArsenals; }
    // }

    [SerializeField] private List<MTState> mts = new List<MTState>();

    public void AddMTState(MTState state) {
        mts.Add(state);
    }

    public List<MTState> MTs {
        get { return mts; }
    }
}
