using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Researcher : MonoBehaviour
{
    [SerializeField] private GameObject notify;

    public void StopResearchingItem(Resources.Arsenal arsenal) {
        Save.rData.SetCurrentlyResearched(arsenal, false);
    }

    public bool ResearchItem(Resources.Arsenal arsenal) {
        int count = 0;
        foreach (var item in Save.rData.CurrentlyResearchingKeys) {
            if (Save.rData[item, 0]) {
                count++;
                if (count >= Save.rData.ReseachSlots)
                    return false;
            }
        }
        Save.rData.SetCurrentlyResearched(arsenal, true);
        return true;
    }

    private void Update() {
        if (Save.rData == null) { enabled = false; return; }
        foreach (var curr in Save.rData.CurrentlyResearchingKeys) {
            if (!Save.rData[curr, 0]) { continue; }
            Save.rData[curr] += Time.deltaTime*1.3f;
            if (Save.rData[curr] >= Resources.GetStatTemplate(curr).maxXp+Gameplay.ResearchXPStep*(Save.rData[curr, true]-1)) {
                Save.rData[curr] = 0f;
                Save.rData[curr, true]++;
                var info = Resources.GetMTInfo(curr);
                if (info.upgradable)
                    Save.rData.AddPowerup(curr, new Powerup(curr, info));
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
