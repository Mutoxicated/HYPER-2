using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Researcher : MonoBehaviour
{
    public void StopResearchingItem(Resources.Arsenal arsenal) {
        Save.rData.SetCurrentlyResearched(arsenal, false);
    }

    public bool ResearchItem(Resources.Arsenal arsenal) {
        if (Save.rData.CurrentlyResearchingCount >= 2) {
            return false;
        }
        Save.rData.SetCurrentlyResearched(arsenal, true);
        return true;
    }

    private void Update() {
        if (Save.rData == null) { enabled = false; }
        foreach (var curr in Save.rData.CurrentlyResearchingKeys) {
            Save.rData[curr] += Time.deltaTime;
            if (Save.rData[curr] >= Resources.GetStatTemplate(curr).maxXp) {
                Save.rData[curr] = 0f;
                Save.rData[curr, true]++;
            }
        }
    }
}
