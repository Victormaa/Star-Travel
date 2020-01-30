using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepPool : AnimalGenericPool<Sheep>
{
    public Sheep Get(int num)
    {
        if(prefebs.Count < num)
        {
            for(int i = 0; i < num; i++)
            {
                AddObjects();
            }
        }
        return prefebs.Dequeue();
    }

    public new static SheepPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
