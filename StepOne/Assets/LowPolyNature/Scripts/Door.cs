using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableItemBase {

    private Animator _animator;


    void Start()
    {
        _animator = GetComponent<Animator>();

    }

    public override bool CanInteract(Collider other)
    { 
        bool result = other is BoxCollider;

        result &= !_animator.GetBool("isopen");

        return result;
    }

    public override void OnInteractAnimation(Animator animator)
    {
        animator.SetTrigger("tr_open");
    }

    public override void OnInteract()
    {
        _animator.SetBool("isopen", true);
        Destroy(GetComponent<BoxCollider>());
    }
}
