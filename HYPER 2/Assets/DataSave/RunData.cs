using System;
using System.Collections.Generic;
using Helpers;
using Unity.VisualScripting;
using UnityEngine;
using static Resources.Arsenal;

[Serializable]
public class RunData
{

    //MACRO_START
    [SerializeField] private int __seed = -1;

    public int _Seed
    {
        get { return __seed; }
    }

    public void SetSeed(int val)
    {
        __seed = val;
    }

    [SerializeField] private int __money = 10;

    public int Money
    {
        get { return __money; }
    }

    public void SetMoney(int val)
    {
        __money = val;
    }

    [SerializeField] private float __xp = 0;

    public float XP
    {
        get { return __xp; }
    }

    public void SetXP(float val)
    {
        __xp = val;
    }

    [SerializeField] private int __lvl = 1;

    public int Lvl
    {
        get { return __lvl; }
    }

    public void SetLvl(int val)
    {
        __lvl = val;
    }
    //MACRO_END
    [SerializeField] private float __linearT = 0.65f;

    public float LinearTime
    {
        get { return __linearT; }
    }

    public void IncLinearTime(float amount)
    {
        __linearT += amount;
    }

    [SerializeField] private int __waves = 1;

    public int Waves
    {
        get { return __waves; }
    }

    public void SetWaves(int waves)
    {
        __waves = waves;
    }

    public void IncrementWaves()
    {
        __waves++;
    }

    [SerializeField]
    private List<Option<Resources.Arsenal>> quickInventoryArsenals = new List<Option<Resources.Arsenal>>() {
        new Option<Resources.Arsenal>(FIRST_MT),
        Option<Resources.Arsenal>.None(),
        Option<Resources.Arsenal>.None(),
        Option<Resources.Arsenal>.None(),
        Option<Resources.Arsenal>.None()
    };

    public List<Option<Resources.Arsenal>> QIAs
    {
        set { quickInventoryArsenals = value; }
        get { return quickInventoryArsenals; }
    }

    [SerializeField] private int selectedIndex = 0;

    public int SelectedIndex
    {
        get { return selectedIndex; }
    }

    public void SetSelectedIndex(int index)
    {
        selectedIndex = index;
    }

    [SerializeField] private List<MTState> mts = new List<MTState>();

    public List<MTState> MTs
    {
        set { mts = value; }
        get { return mts; }
    }

    [SerializeField] private int firstCorner = 0;
    public int FirstCorner
    {
        get { return firstCorner; }
    }
    public void RandomizeCorner()
    {
        firstCorner = Seed.IntRange(1, 5);
    }

    [SerializeField] private List<PathAlt> paths = new List<PathAlt>();
    [SerializeField] private List<int> widenedPaths = new List<int>();

    public List<PathAlt> Paths
    {
        set { paths = value; }
        get { return paths; }
    }

    public List<int> WidenedPaths
    {
        set { widenedPaths = value; }
        get { return widenedPaths; }
    }

    [SerializeField] private List<ArsenalKey> arsenalKeys = new List<ArsenalKey>()
    {
        new ArsenalKey(FIRST_MT, false, true)
    };

    public List<ArsenalKey> ArsenalKeys
    {
        set { arsenalKeys = value; }
        get { return arsenalKeys; }
    }
    [SerializeField] private List<int> researchLvls = new List<int>()
    {
        1
    };
    public List<int> ResearchLvls
    {
        set { researchLvls = value; }
        get { return researchLvls; }
    }

    [SerializeField] private List<float> researchProgressValues = new List<float>()
    {
        0f
    };

    public List<float> ResearchProgressValues
    {
        set { researchProgressValues = value; }
        get { return researchProgressValues; }
    }

    [SerializeField] private List<Upgrades> takenUpgrades = new List<Upgrades>()
    {
        new Upgrades()
    };
    public List<Upgrades> TakenUpgrades
    {
        set { takenUpgrades = value; }
        get { return takenUpgrades; }
    }

    [SerializeField] private List<Powerups> pendingPowerups = new List<Powerups>()
    {
        new Powerups()
    };

    public List<Powerups> Powerups
    {
        set { pendingPowerups = value; }
        get { return pendingPowerups; }
    }

    [SerializeField] private int __researchSlots = 2;

    public int ReseachSlots
    {
        set { __researchSlots = value; }
        get { return __researchSlots; }
    }

    // ENEMY CHANCE EVALUATOR RELATED
    [SerializeField] private List<float> enemyChanceTimes = new List<float>();
    public List<float> EnemyChanceTimes
    {
        set { enemyChanceTimes = value; }
        get { return enemyChanceTimes; }
    }
    [SerializeField] private List<float> enemyChanceSteps = new List<float>();
    public List<float> EnemyChanceSteps {
        set { enemyChanceSteps = value; }
        get { return enemyChanceSteps; }
    }
}

[Serializable]
public class Powerups {
    public List<Powerup> self = new List<Powerup>();

    public Powerups() {
        self = new List<Powerup>();
    }
}

[Serializable]
public class Upgrades {
    public List<Resources.MTUpgradeType> self = new List<Resources.MTUpgradeType>();

    public Upgrades() {
        self = new List<Resources.MTUpgradeType>();
    }
}

[Serializable]
public class ArsenalKey
{
    public Resources.Arsenal type;
    public bool researchActive = false;
    public bool unlocked = false;

    public ArsenalKey(Resources.Arsenal type, bool active, bool unlocked)
    {
        this.type = type;
        this.researchActive = active;
        this.unlocked = unlocked;
    }
}