using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TestSpider : MonoBehaviour
{
    private enum SheepState
    {
        Patrol,
        Chase,
        Attack,
        Idle
    };

    private SheepState State = SheepState.Patrol;

    private AIPath mAIAgent;

    private AIDestinationSetter DestinationSetter;

    private Animator mAnimator;

    public GameObject Player;

    public float EnemyDistanceChase = 5.0f;

    private float EnemyDistanceAttack = 2.0f;

    private bool mIsDead = false;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mAIAgent = this.transform.parent.GetComponent<AIPath>();
        DestinationSetter = this.transform.parent.GetComponent<AIDestinationSetter>();
    }

    private bool IsNavMeshMoving
    {
        get
        {
            return mAIAgent.velocity.magnitude > 0.1f;
        }
    }

    private float sqrtDistance;
    private float enemyDistanceAttackSqrt;

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
    }

    private void OnPatrol()
    {
        canMove = true;
        mAnimator.SetBool("isAttacking", false);
    }

    private void OnAttack()
    {
        isAttackFinish = false;
        canMove = false;

        AnimatorStateInfo info = mAnimator.GetCurrentAnimatorStateInfo(0);
        if(info.normalizedTime >= 1.0f)
        {
            isAttackFinish = true;
        }

        //this.transform.LookAt(Player.transform, this.transform.up);
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyDistanceAttack = mAIAgent.endReachedDistance+0.1f;
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
            mAIAgent.canMove = true;
            mAIAgent.maxSpeed = 2;
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
        if(sqrtDistance >= EnemyDistanceChaseSqrt && isAttackFinish)
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

    #region new area
    public bool canMove = true;
    public bool isAttackFinish = true;
    #endregion

    #region
    public void ReplaceTarget()
    {
        if(DestinationSetter.target != null)
        {
            DestinationSetter.target = null;
        }
        else
        {
            DestinationSetter.target = Player.transform;
        }
        
    }

    public void rebackData()
    {
        
    }
    #endregion
}
