using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cannon : InteractableItemBase
{
    private Animator _animator;

    private bool mIsLoaded = false;

    private Transform CannonBall_Spawn = null;

    public GameObject Cannonball = null;

    private ParticleSystem PS_Smoke;

    public float Power = 12.0f;

    public bool Interactable = true;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        CannonBall_Spawn = transform.Find("CannonBall_Spawn");

        PS_Smoke = transform.Find("PS_Smoke").GetComponent<ParticleSystem>();
    }

    public void Shoot()
    {
        GameObject cannonball = Instantiate(Cannonball, 
            CannonBall_Spawn.position, Quaternion.identity);

        Rigidbody rb = cannonball.AddComponent<Rigidbody>();

        rb.velocity = Power * CannonBall_Spawn.forward;

        StartCoroutine(RemoveCannonBall_Rigidbody(rb, 3.0f));

        _animator.SetTrigger("tr_shoot");

        PS_Smoke.Play();
    }

    IEnumerator RemoveCannonBall_Rigidbody(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(rb);
    }

    public override void OnInteractAnimation(Animator animator)
    {
        // Animator of the player
        animator.SetTrigger("tr_open");
    }

    public override bool CanInteract(Collider other)
    {
        if (!Interactable)
            return false;

        PlayerController player = GameManager.Instance.Player;
        if (!player.CarriesItem("Cannonball") && !mIsLoaded)
            return false;

        if(!mIsLoaded)
            InteractText = "Press F to load it";
        else
            InteractText = "Press F to shoot";

        return true;
    }

    public override void OnInteract()
    {
        if (!mIsLoaded)
        {
            _animator.SetTrigger("tr_load");

            // Drop the cannonball and destroy it
            // It is just needed to load the cannon
            PlayerController player = GameManager.Instance.Player;
            player.DropAndDestroyCurrentItem();

            mIsLoaded = true;
        }
        else
        {


            Shoot();

            mIsLoaded = false;
        }
    }
}
