using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject holder;
    [SerializeField] private Field tField;
    [SerializeField] private Field enemyLvl;

    private void Update() {
        if (player.OnDebugPanel) {
            holder.SetActive(!holder.activeInHierarchy);
        }
        tField.ChangeValue(String.Format("{0}:{1}, {2}:{3}", Gameplay.LinearT, Gameplay.IntLinearT, Gameplay.AsymT,  Gameplay.IntAsymT));
        enemyLvl.ChangeValue(Gameplay.EnemyLvl);
    }
}
