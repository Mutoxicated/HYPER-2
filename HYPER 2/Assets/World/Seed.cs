using UnityEngine;

public class Seed : MonoBehaviour
{
    public static System.Random random;

    public void DStart()
    {
        if (Save.rData._Seed == -1){
            Save.rData.SetSeed(Random.Range(0,999999999));
        }
        random = new System.Random(Save.rData._Seed);
    }

    public static int IntRange(int min, int max) {
        int minINT = Mathf.RoundToInt(min*100f);
        int maxINT = Mathf.RoundToInt(max*100f);
        return random.Next(minINT, maxINT)/100;
    }

    public static float NextFloat(float min, float max){
        int minINT = Mathf.RoundToInt(min*100f);
        int maxINT = Mathf.RoundToInt(max*100f);
        return random.Next(minINT,maxINT)/100f;
    }

    public static bool Chance(float prc) {
        var random = NextFloat(0,100);
        return random < prc;
    }
}
