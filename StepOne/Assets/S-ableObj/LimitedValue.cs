using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LimitedValue : ScriptableObject
{
    public float _limitedValue;

    public float Value
    {
        get
        {
            return _limitedValue;
        }
        set
        {
            if (_limitedValue < 0)
            {
                _limitedValue = 0;
            }
            else if (_limitedValue > 100)
            {
                _limitedValue = 100;
            }
            else
            {
                _limitedValue = value;
            }
        }
    }
}
