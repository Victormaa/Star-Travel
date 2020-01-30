using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGuy : MonoBehaviour, IDamageable {

    private bool mIsDead = false;

    public void TakeDamage(int amount)
    {
        if (!mIsDead)
        {
            GetComponent<Animator>().SetTrigger("death");
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<BoxCollider>());
            mIsDead = true;
        }
    }

    public bool IsDead
    {
        get { return mIsDead; }
    }


}
