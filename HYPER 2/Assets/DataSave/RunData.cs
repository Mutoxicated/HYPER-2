using System;

[Serializable]
public class RunData
{
    //MACRO_START
    public int __seed = -1;
    
    public int Seed {
    	get { return __seed; }
    }
    
    public void SetSeed(int val) {
    	__seed = val;
    }
    //MACRO_END
}
