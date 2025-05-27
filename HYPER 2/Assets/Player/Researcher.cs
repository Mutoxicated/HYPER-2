using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Researcher : MonoBehaviour
{
    [SerializeField] private GameObject notify;
    private List<int> researchLvls;
    private List<float> currentlyResearchingValues;

    private Arsenal arsenal;
    private PowerupManager pm;
    private int occupiedResearchSlots;
    private int researchSlots;

    public void DStart(Arsenal arsenal, PowerupManager pm)
    {
        this.pm = pm;
        this.arsenal = arsenal;
        for (int i = 0; i < arsenal.Keys.Count; i++)
        {
            if (!arsenal.Keys[i].unlocked)
            {
                break;
            }
            if (arsenal.Keys[i].researchActive)
            {
                occupiedResearchSlots++;
            }
        }

        currentlyResearchingValues = Save.rData.ResearchProgressValues;
        researchLvls = Save.rData.ResearchLvls;
        researchSlots = Save.rData.ReseachSlots;

        PublicEvent.onSaveRun += data =>
        {
            Save.rData.ResearchProgressValues = currentlyResearchingValues;
            Save.rData.ReseachSlots = researchSlots;
        };
    }
    public void InitializeArsenalTypeData(Resources.Arsenal arsenal)
    {
        researchLvls.Add(1);
        currentlyResearchingValues.Add(0);
    }

    public float GetResearchLvl(Resources.Arsenal arsenal)
    {
        return researchLvls[(int)arsenal];
    }

    public float GetResearchValue(Resources.Arsenal arsenal)
    {
        return currentlyResearchingValues[(int)arsenal];
    }

    public void IncResearchSlots()
    {
        researchSlots++;
    }

    public void StopResearchingItem(Resources.Arsenal arsenalType)
    {
        arsenal.Keys[(int)arsenalType].researchActive = false;
    }

    public bool ResearchItem(Resources.Arsenal arsenalType) {
        if (occupiedResearchSlots >= researchSlots) {
            return false;
        }
        arsenal.Keys[(int)arsenalType].researchActive = true;
        return true;
    }

    private void Update() {
        if (Save.rData == null) { enabled = false; return; }
        foreach (var curr in arsenal.Keys) {
            if (!curr.researchActive) { continue; }
            var value = currentlyResearchingValues[(int)curr.type] += Time.deltaTime*1.3f;
            if (value >= Resources.GetStatTemplate(curr.type).maxXp+Gameplay.ResearchXPStep*(Mathf.FloorToInt(researchLvls[(int)curr.type])-1)) {
                currentlyResearchingValues[(int)curr.type] = 0f;
                researchLvls[(int)curr.type]++;
                var info = Resources.GetMTInfo(curr.type);
                if (info.upgradable)
                    pm.AddPowerup(curr.type, info);
            }
        }
        foreach (var pows in Save.rData.Powerups) {
            if (pows.self.Count != 0) {
                notify.SetActive(true);
                return;
            }
        }
        notify.SetActive(false);
    }
}
