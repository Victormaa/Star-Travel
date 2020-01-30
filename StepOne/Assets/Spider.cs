using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    private enum SheepState
    {
        Patrol,
        Chase,
        Attack,
        Idle
    };

    private SheepState State = SheepState.Patrol;

    private AIPathAlignedToSurface mAIAgent;

    [SerializeField]
    private Patrol _Patrol;

    [SerializeField]
    private AIDestinationSetter DestinationSetter;

    private Animator mAnimator;

    public GameObject Player;

    public bool canMove = true;

    public bool isAttackFinish = true;

    public float EnemyDistanceChase = 5.0f;

    public float EnemyDistanceAttack = 2.0f;

    private bool mIsDead = false;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        mAIAgent = GetComponent<AIPathAlignedToSurface>();
        mAnimator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool IsNavMeshMoving
    {
        get
        {
            return mAIAgent.velocity.magnitude > 0.1f;
        }
    }

    private float sqrtDistance = 100;
    private float enemyDistanceAttackSqrt = 20;

    private bool IsAttacking
    {
        get
        {
            return sqrtDistance < enemyDistanceAttackSqrt;
        }
    }
   

    private void Chase()
    {
        canMove = true;
        mAnimator.SetBool("isAttacking", false);
        _Patrol.enabled = false;
        DestinationSetter.enabled = true;
        DestinationSetter.target = Player.transform;
    }

    private void OnPatrol()
    {
        canMove = true;
        mAnimator.SetBool("isAttacking", false);
        DestinationSetter.enabled = false;
        _Patrol.enabled = true;       
    }

    private void OnAttack()
    {
        isAttackFinish = false;
        canMove = false;

        AnimatorStateInfo info = mAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 1.0f)
        {
            isAttackFinish = true;
        }
        //this.transform.LookAt(Player.transform, this.transform.up);
    }
    // Update is called once per frame
    void Update()
    {
        if (mIsDead)
            return;
        if (!canMove)
        {
            mAIAgent.maxSpeed = 0;
        }
        else
        {
            //mAIAgent.canMove = true;
            mAIAgent.maxSpeed = 4;
        }

        sqrtDistance = (transform.position - Player.transform.position).sqrMagnitude;
        enemyDistanceAttackSqrt = EnemyDistanceAttack * EnemyDistanceAttack;

        float EnemyDistanceChaseSqrt = EnemyDistanceChase * EnemyDistanceChase;

        if (sqrtDistance < EnemyDistanceChaseSqrt && isAttackFinish)
        {
            State = SheepState.Chase;
            if (sqrtDistance <= enemyDistanceAttackSqrt)
            {
                State = SheepState.Attack;
            }
        }
        if (sqrtDistance >= EnemyDistanceChaseSqrt && isAttackFinish)
        {
            State = SheepState.Patrol;
        }

        switch (State)
        {
            case SheepState.Idle:               
                return;
            case SheepState.Chase:
                mAnimator.SetBool("isWalking", IsNavMeshMoving);
                Chase();
                return;
            case SheepState.Attack:
                mAnimator.SetBool("isWalking", false);
                mAnimator.SetBool("isAttacking", IsAttacking);                
                OnAttack();//hp down
                return;
            case SheepState.Patrol:
                mAnimator.SetBool("isWalking", IsNavMeshMoving);
                OnPatrol();
                return;
            default:
                break;
        }
    }
}
