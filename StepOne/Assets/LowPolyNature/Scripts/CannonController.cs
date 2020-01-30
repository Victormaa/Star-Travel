using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour {

    public float ShootRepeatRate = 0.0f;

    public float ShootDelay = 0.0f;

    public bool ShootOnStart = false;

    private Cannon CannonScript;

    void Start()
    {
        CannonScript = GetComponent<Cannon>();

        if(ShootOnStart)
        {
            Invoke("Shoot", ShootDelay);
        }

        StartTimer();
    }

    public void Shoot()
    {
        if (CannonScript != null)
        {
            CannonScript.Shoot();
        }
    }

    public void StartTimer()
    {
        if (ShootRepeatRate > 0.0f)
        {
            InvokeRepeating("Shoot", ShootDelay, ShootRepeatRate);
        }
    }

    public void StopTimer()
    {
        CancelInvoke("Shoot");
    }



}
