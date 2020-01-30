using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPool : AnimalGenericPool<Spider>
{
    public Spider Get(int num)
    {
        if (prefebs.Count < num)
        {
            for (int i = 0; i < num; i++)
            {
                AddObjects();
            }
        }
        return prefebs.Dequeue();
    }

    public new static SpiderPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
