using UnityEngine;
using Globals;
using EnumType;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemySight))]
public class Enemy : MonoBehaviour, IDamageable
{
	[HideInInspector] public Rigidbody2D rb;
	[HideInInspector] public Animator anim;

	private EnemyState enemyState;
	private EnemyStatsRuntime enemyStatsRuntime;

	// Components
	private EnemyMovement movement;
	private EnemyAttack attack;
	private EnemySight sight;
	private EnemyDataManager dataManager;

	// State timers & helper variables
	private float idleTime;
	private float idleTimer;
	private float patrolTime;
	private float patrolTimer;
	private bool isPatrolling;
	private float chaseTime = 2f;
	private float chaseTimer;
	private float shootTime = 1.0f;
	private float shootTimer;
	private Coroutine delayAttackCoroutine;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		movement = GetComponent<EnemyMovement>();
		attack = GetComponent<EnemyAttack>();
		sight = GetComponent<EnemySight>();
		dataManager = GetComponent<EnemyDataManager>();
	}

	private void Start()
	{
		// 플레이어 스탯 초기화
		enemyStatsRuntime = new EnemyStatsRuntime(dataManager._enemyStats);

		enemyState = EnemyState.IDLE;
		ChangeState(enemyState);	// 초기 상태 설정
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

		if (!isPatrolling)
		{
			movement.Flip();
			isPatrolling = true;
		}

		Vector2 dir = movement.GetFacingDirection();
		movement.Move(dir, dataManager._enemyStats.PatrolSpeed);

		patrolTimer += Time.deltaTime;
		if (patrolTimer >= patrolTime)
		{
			movement.StopHorizontal();
			ChangeState(EnemyState.IDLE);
		}
	}

	private void UpdateChase()
	{
		chaseTimer += Time.deltaTime;
		Transform player = GameManager.Instance.playerObj != null ? GameManager.Instance.playerObj.transform : null;

		bool playerVisible = sight.IsPlayerInRange();
		if (playerVisible)
		{
			chaseTimer = 0f;
		}
		else if (chaseTimer >= chaseTime)
		{
			ChangeState(EnemyState.IDLE);
			return;
		}

		if (player != null)
		{
			Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
			movement.Move(dir, dataManager._enemyStats.ChaseSpeed);

			float dist = Vector2.Distance(transform.position, player.position);
			if (dist <= dataManager._enemyStats.AttackRange)
			{
				movement.StopHorizontal();
				ChangeState(EnemyState.ATTACK);
			}
		}
	}

	private void UpdateAttack()
	{
		shootTimer += Time.deltaTime;
		movement.StopHorizontal();

		if (shootTimer >= shootTime)
		{
			attack.FirePoolBullet();
			shootTimer = 0f;
		}

		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		if (!stateInfo.IsName(EnemyAnimName.attack))
		{
			anim.Play(EnemyAnimName.recharge);

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
				anim.Play(EnemyAnimName.idle);
				idleTime = Random.Range(3f, 4f);
				idleTimer = 0f;
				movement.StopHorizontal();
				break;

			case EnemyState.PATROL:
				anim.Play(EnemyAnimName.patrol);
				isPatrolling = false;
				patrolTimer = 0f;
				patrolTime = Random.Range(2f, 3f);
				break;

			case EnemyState.CHASE:
				anim.Play(EnemyAnimName.chase);
				chaseTimer = 0f;
				chaseTime = 2f;
				delayAttackCoroutine = StartCoroutine(DelayAttack());
				break;

			case EnemyState.ATTACK:
				anim.Play(EnemyAnimName.attack);
				shootTimer = 0f;
				movement.StopHorizontal();
				break;
		}
	}

	// 데미지 처리
	public void TakeDamage(int attack)
	{
		enemyStatsRuntime.CurrentHP -= attack;

		if (enemyStatsRuntime.CurrentHP <= 0)
		{
			GameManager.Instance.poolManager.ReturnToPool(gameObject);
			Debug.Log($"적 사망 (피해량: {attack})");
		}
	}

	private IEnumerator DelayAttack()
	{
		FindBangUI bangUI = GetComponentInChildren<FindBangUI>();
		if (bangUI != null)
		{
			bangUI.VisibleUI();
		}
		yield return new WaitForSeconds(dataManager._enemyStats.FindCoolTime);
		if (bangUI != null)
		{
			bangUI.DisableUI();
		}
	}
}
