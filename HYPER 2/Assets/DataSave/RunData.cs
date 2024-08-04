using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static Resources.Arsenal;

[Serializable]
public class RunData
{
    //MACRO_START
    [SerializeField] private int __seed = -1;
    
    public int _Seed {
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
    [SerializeField] private float __linearT = 0.5f;

    public float LinearTime {
        get { return __linearT; }
    }
    
    public void IncLinearTime(float amount) {
        __linearT += amount;
    }

    // [SerializeField] private int __runTimeSeconds = 0;

    // public int RunTimeSecs {
    //     get { return __runTimeSeconds; }
    // }
    
    // public void IncRunTime() {
    //     __runTimeSeconds++;
    // }

    [SerializeField] private int __waves = 1;

    public int Waves {
        get { return __waves; }
    }

    public void SetWaves(int waves) {
        __waves = waves;
    }

    public void IncrementWaves() {
        __waves++;
    }

    [SerializeField] private List<Option<Resources.Arsenal>> quickInventoryArsenals = new List<Option<Resources.Arsenal>>() {
        new Option<Resources.Arsenal>(FIRST_MT)
    };

    public void UpdateQIACapacity(int capacity) {
        quickInventoryArsenals.Capacity = capacity;
        for (int i = 0; i <= capacity; i++) {
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

    [SerializeField] private int firstCorner = 0;
    public int FirstCorner {
        get { return firstCorner; }
    }
    public void RandomizeCorner() {
        firstCorner = Seed.IntRange(1, 5);
    }

    [SerializeField] private List<PathAlt> paths = new  List<PathAlt>();
    [SerializeField] private List<int> pathsToWiden = new List<int>();

    public void AddPath(List<Neighbor> nbs) {
        paths.Add(new PathAlt(nbs));
    }

    public void AddPathToWiden(int index) {
        pathsToWiden.Add(index);
    }

    public List<PathAlt> Paths {
        get { return paths; }
    }

    public List<int> PathsToWiden {
        get { return pathsToWiden; }
    }

    [SerializeField] private List<Resources.Arsenal> researchProgressKeys = new List<Resources.Arsenal>() {
        BOOMER,
        FIRST_MT,
        SECOND_MT,
        WATCHER,
        RADMOL,
    };

    [SerializeField] private List<int> researchProgressValues = new List<int>() {
        1,
        1,
        1,
        1,
        1,
    };

    [SerializeField] private List<(bool, float)> currentlyResearchingValues = new List<(bool, float)>() {
        (false, 0f),
        (false, 0f),
        (false, 0f),
        (false, 0f),
        (false, 0f),
    };

    public List<Resources.Arsenal> CurrentlyResearchingKeys {
        get { return researchProgressKeys; }
    }

    public int  CurrentlyResearchingCount {
        get { 
            return currentlyResearchingValues.Count;
        }
    }

    public int this[Resources.Arsenal arsnl, bool lvl] {
        get {
            for (int i = 0; i < researchProgressKeys.Count; i++) {
                if (arsnl == researchProgressKeys[i]) {
                    return researchProgressValues[i];
                }
            }
            return 0;
        }
        set {
            for (int i = 0; i < researchProgressKeys.Count; i++) {
                if (arsnl == researchProgressKeys[i]) {
                    researchProgressValues[i] = value;
                }
            }
        }
    }

    public float this[Resources.Arsenal arsnl] {
        get {
            for (int i = 0; i < researchProgressKeys.Count; i++) {
                if (arsnl == researchProgressKeys[i]) {
                    return currentlyResearchingValues[i].Item2;
                }
            }
            return -1f;
        }
        set {
            for (int i = 0; i < researchProgressKeys.Count; i++) {
                if (arsnl == researchProgressKeys[i]) {
                    currentlyResearchingValues[i] = (currentlyResearchingValues[i].Item1, value);
                }
            }
        }
    }

    public bool this[Resources.Arsenal arsnl, int _] {
        get {
            for (int i = 0; i < researchProgressKeys.Count; i++) {
                if (arsnl == researchProgressKeys[i]) {
                    return currentlyResearchingValues[i].Item1;
                }
            }
            return false;
        }
    }

    public void SetCurrentlyResearched(Resources.Arsenal arsnl, bool activated) {
        for (int i = 0; i < researchProgressKeys.Count; i++) {
            if (arsnl == researchProgressKeys[i]) {
                currentlyResearchingValues[i] = (activated, currentlyResearchingValues[i].Item2);
                break;
            }
        }
    }
}

[Serializable]
public class PathState 
{
    public Branch root1;
    public Branch root2;
}