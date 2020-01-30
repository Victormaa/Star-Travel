using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testplayer : MonoBehaviour, IDamageable
{
    public TestInjuredEffect HealthBar;

    public FloatReference Hitpoints;

    [SerializeField]
    private FloatReference FoodPoints;

    [Tooltip("Rate in seconds in which the hunger increases")]
    public float HungerRate = 0.5f;

    public void TakeDamage(int amount)
    {
        Hitpoints.Value -= amount;
        HealthBar.damaged = true;
    }

    public void TakeHunger()
    {
        FoodPoints.Value--;
        if(FoodPoints.Value == 0)
        {
            CancelInvoke();
        }
    }

    private void Start()
    {
        InvokeRepeating("TakeHunger", 0, HungerRate);
    }

    public bool IsDead
    {
        get
        {
            return Hitpoints.Value == 0;
        }
    }

}
