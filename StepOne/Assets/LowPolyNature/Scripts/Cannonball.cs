using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : InventoryItemBase
{
    private bool mIsFlying = false;

    public bool IsFlying
    {
        get
        {
            return mIsFlying;
        }

        set
        {
            mIsFlying = value;
        }
    }

    public int DamageAmount = 100;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var collisionScript in 
            collision.gameObject.GetComponents<MonoBehaviour>())
        {
            if (collisionScript is IDamageable)
            {
                (collisionScript as IDamageable).TakeDamage(DamageAmount);
            }
        }
    }


}
