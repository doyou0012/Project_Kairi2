using System.Collections;
using EnumType;
using Globals;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyLongRangeAttack))]
[RequireComponent(typeof(EnemySight))]
[RequireComponent(typeof(EnemyPatrolBehavior))]
[RequireComponent(typeof(EnemyChaseBehavior))]
public class EnemyBrain : MonoBehaviour
{
    private EnemyState enemyState = EnemyState.IDLE;
    public EnemyState CurrentState => enemyState;

    private EnemyMovement movement;
    private EnemyLongRangeAttack attack;
    private EnemySight sight;
    private EnemyDataManager dataManager;
    private EnemyPatrolBehavior patrolBehavior;
    private EnemyChaseBehavior chaseBehavior;
    private Animator anim;

    // Idle state timers
    private float idleTime;
    private float idleTimer;

    private Coroutine delayAttackCoroutine;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyLongRangeAttack>();
        sight = GetComponent<EnemySight>();
        dataManager = GetComponent<EnemyDataManager>();
        patrolBehavior = GetComponent<EnemyPatrolBehavior>();
        chaseBehavior = GetComponent<EnemyChaseBehavior>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        enemyState = EnemyState.IDLE;
        ChangeState(enemyState);
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.IDLE:
                UpdateIdle();
                break;
            case EnemyState.PATROL:
                UpdatePatrol();
                break;
            case EnemyState.CHASE:
                UpdateChase();
                break;
            case EnemyState.ATTACK:
                UpdateAttack();
                break;
        }
    }

    private void UpdateIdle()
    {
        if (sight.IsPlayerInRange())
        {
            ChangeState(EnemyState.CHASE);
            return;
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime)
        {
            ChangeState(EnemyState.PATROL);
        }
    }

    private void UpdatePatrol()
    {
        if (sight.IsPlayerInRange())
        {
            ChangeState(EnemyState.CHASE);
            return;
        }

        bool finished = patrolBehavior.UpdatePatrol(movement, dataManager);
        if (finished)
        {
            ChangeState(EnemyState.IDLE);
        }
    }

    private void UpdateChase()
    {
        Transform player = GameManager.Instance.playerObj != null ? GameManager.Instance.playerObj.transform : null;
        ChaseResult result = chaseBehavior.UpdateChase(player, movement, sight, dataManager);

        switch (result)
        {
            case ChaseResult.TargetLost:
                ChangeState(EnemyState.IDLE);
                break;
            case ChaseResult.InAttackRange:
                ChangeState(EnemyState.ATTACK);
                break;
            case ChaseResult.Chasing:
                break;
        }
    }

    private void UpdateAttack()
    {
        bool attackFinished = attack.UpdateAttack(movement, anim);
        if (attackFinished)
        {
            if (sight.IsPlayerInRange())
            {
                ChangeState(EnemyState.CHASE);
            }
            else
            {
                ChangeState(EnemyState.IDLE);
            }
        }
    }

    public void ChangeState(EnemyState p_state)
    {
        // Exit current state behavior
        switch (enemyState)
        {
            case EnemyState.CHASE:
                if (delayAttackCoroutine != null)
                {
                    StopCoroutine(delayAttackCoroutine);
                    delayAttackCoroutine = null;
                }
                FindBangUI bangUI = GetComponentInChildren<FindBangUI>();
                if (bangUI != null)
                {
                    bangUI.DisableUI();
                }
                break;
        }

        enemyState = p_state;

        // Enter new state behavior
        switch (enemyState)
        {
            case EnemyState.IDLE:
                if (anim != null) anim.Play(EnemyAnimName.idle);
                idleTime = Random.Range(3f, 4f);
                idleTimer = 0f;
                movement.StopHorizontal();
                break;

            case EnemyState.PATROL:
                if (anim != null) anim.Play(EnemyAnimName.patrol);
                patrolBehavior.OnEnterPatrol();
                break;

            case EnemyState.CHASE:
                if (anim != null) anim.Play(EnemyAnimName.chase);
                chaseBehavior.OnEnterChase();
                delayAttackCoroutine = StartCoroutine(DelayAttack());
                break;

            case EnemyState.ATTACK:
                if (anim != null) anim.Play(EnemyAnimName.attack);
                attack.OnEnterAttack();
                movement.StopHorizontal();
                break;
        }
    }

    private IEnumerator DelayAttack()
    {
        FindBangUI bangUI = GetComponentInChildren<FindBangUI>();
        if (bangUI != null)
        {
            bangUI.VisibleUI();
        }

        float coolTime = (dataManager != null && dataManager._enemyStats != null) ? dataManager._enemyStats.FindCoolTime : 0.5f;
        yield return new WaitForSeconds(coolTime);

        if (bangUI != null)
        {
            bangUI.DisableUI();
        }
    }
}
