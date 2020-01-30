using UnityEngine;

public class InjureTarget : MonoBehaviour
{
    public float health;

    public void Takedamage(float amount)
    {
        health -= amount;

        if(health <= 0)
        {
            Died();
        }
    }

    public void Died()
    {
        Destroy(gameObject);
    }
}
