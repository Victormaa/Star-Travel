using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private enum SheepState
    {
        Patrol,
        Escape
    };

    private SheepState State = SheepState.Patrol;

    //private NavMeshAgent mAgent;
    private AIPathAlignedToSurface mAIAgent;

    [SerializeField]
    private Patrol _Patrol;
    [SerializeField]
    private AIDestinationSetter DestinationSetter;

    private bool isRunAway = false;

    private Animator mAnimator;

    public GameObject Player;

    public float EnemyDistanceRun = 4.0f;

    private bool mIsDead = false;

    public GameObject[] ItemsDeadState = null;

    public Transform NewPos;

    //public Transform PatrolPoints;

    // Use this for initialization

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        mAIAgent = GetComponent<AIPathAlignedToSurface>();
        mAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        //mAgent = GetComponent<NavMeshAgent>();       
    }

    //private void OnDisable()
    //{
    //    this.gameObject.GetComponent<AIPathAlignedToSurface>().enabled = false;
    //    this.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
    //    this.gameObject.GetComponent<FunnelModifier>().enabled = false;
    //    this.gameObject.GetComponent<Seeker>().enabled = false;
    //    this.gameObject.GetComponent<Patrol>().enabled = false;
    //    SheepPool.Instance.ReturnToPool(this);
    //}

    private void OnEnable()
    {
        mIsDead = false;
        OnReload();
    }

    //private void OnEnable()
    //{
    //    Patrol = this.GetComponent<Patrol>();
    //    for (int i = 0; i < Patrol.targets.Length; i++)
    //    {
    //        var target = Patrol.targets[i];
    //        if (!target.gameObject)
    //        {
    //            var min = 0;

    //            var max = 13;

    //            target = PatrolPoints.GetChild(Random.Range(min, max));
    //        } 
    //    }
    //}

    private bool IsNavMeshMoving
    {
        get
        {
            //return mIsDead;   ///?????? remove this stupid thing
            return mAIAgent.velocity.magnitude > 0.1f;
            //return mAgent.velocity.magnitude > 0.1f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        InteractableItemBase item = collision.collider.gameObject.GetComponent<InteractableItemBase>();
        if(item != null)
        {
            // Hit by a weapon
            if(item.ItemType == EItemType.Weapon)
            {
                if(Player.GetComponent<PlayerController>().IsAttacking)
                {
                    
                    mAIAgent.enabled = false;
                    //mAgent.enabled = false;
                    mIsDead = true;
                    OnDead();
                    mAnimator.SetTrigger("die");
                    //Destroy(GetComponent<Rigidbody>());
                    Invoke("ShowItemsDeadState", 0.8f);
                }
            }
        }
    }

    private void OnDead()
    {
        SheepPool.Instance.ReturnToPool(this);
    }

    private void OnReload()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        transform.Find("sheep_mesh").GetComponent<SkinnedMeshRenderer>().enabled = true;
        mAIAgent.enabled = true;
    }

    void ShowItemsDeadState()
    {
        // Activate the items
        foreach(var item in ItemsDeadState)
        {
            var theItem = Instantiate(item);
            theItem.transform.position = item.transform.position;
            theItem.SetActive(true);
            //item.SetActive(true);
        }

        //Destroy(GetComponent<CapsuleCollider>());
        GetComponent<CapsuleCollider>().enabled = false;

        // Hide the sheep mesh
        transform.Find("sheep_mesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (mIsDead)
            return;

        // Only runaway if player is armed
        bool isPlayerArmed = Player.GetComponent<PlayerController>().IsArmed;

        float squaredDist = (transform.position - Player.transform.position).sqrMagnitude;
        float EnemyDistanceRunSqrt = EnemyDistanceRun * EnemyDistanceRun;

        //Debug.Log(isPlayerArmed);
        // Run away from player
        if (squaredDist < EnemyDistanceRunSqrt && isPlayerArmed)
        {
            isRunAway = true;
            _Patrol.enabled = false;
            DestinationSetter.enabled = true;

            Vector3 dirToPlayer = transform.position - Player.transform.position;

            Vector3 newPos = transform.position + dirToPlayer;

            NewPos.position = newPos;

            this.GetComponent<AIDestinationSetter>().target = NewPos;
        }
        else
        {
            if (isRunAway)
            {
                isRunAway = false;
                _Patrol.enabled = true;
                DestinationSetter.enabled = false;
            }
        }

        mAnimator.SetBool("walk", IsNavMeshMoving);

    }
}
