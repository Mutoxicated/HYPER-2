using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECEDebugger : MonoBehaviour
{
    [SerializeField] private GameObject fieldPrefab;
    [SerializeField] private EnemyChanceEvaluator ece;

    private List<Field> fields = new List<Field>();

    private void Update() {
        for (int i = 0; i < Save.rData.EnemyKeys.Count; i++) {
            if (!ece.sinWaveModifiers.ContainsKey(Save.rData.EnemyKeys[i])) {
                continue;
            }
            if (fields.Count < i+1) {
                var field = Instantiate(fieldPrefab, transform.position-Vector3.down*(fields.Count+1), Quaternion.identity).GetComponent<Field>();
                field.ChangeName(Save.rData.EnemyKeys[i].ToString());
                field.CStart();
                break;
            }
            fields[i].ChangeValue(ece.GetEnemyChance(Save.rData.EnemyKeys[i]));
        }
    }
}
