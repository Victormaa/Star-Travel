using System;

[Serializable]
public class oneValue
{
    public LimitedValue Variable;
   
    public float Value
    {
        get
        {
            return Variable.Value;
        }
        set
        {
            Variable.Value = value;
        }
    }
}
