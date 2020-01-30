using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FauxGravityBody : MonoBehaviour
{
    public float customGravity;

    public FauxGravityAttractor planet;

    private Transform myTransform;

    private Rigidbody Rbody;

    public Rigidbody RBody
    {
        get { return Rbody; }
    }

    private bool OnPlanet = true;

    public void StayOnPlanet()
    {
        OnPlanet = true;
        // freeze rotation Dont use gravity;
        Rbody.useGravity = false;
        Rbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void OffPlanet()
    {
        OnPlanet = false;
        // dont freeze rotation use gravity;
        Rbody.useGravity = true;
        Rbody.constraints = RigidbodyConstraints.None;
    }

    private void Awake()
    {
        //planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<FauxGravityAttractor>();
        myTransform = this.transform;
        InitialRbody();
    }

    private void InitialRbody()
    {
        Rbody = this.GetComponent<Rigidbody>();
        Rbody.useGravity = false;
        Rbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        if (OnPlanet)
        {
            planet.Attract(myTransform, customGravity);
        }
        
    }
}
