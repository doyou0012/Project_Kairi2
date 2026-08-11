using UnityEngine;
using Globals;
using EnumType;

[RequireComponent(typeof(EnemyBrain))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyLongRangeAttack))]
[RequireComponent(typeof(EnemySight))]
[RequireComponent(typeof(EnemyDataManager))]
public class Enemy : MonoBehaviour, IDamageable
{
	[HideInInspector] public Rigidbody2D rb;
	[HideInInspector] public Animator anim;

	private EnemyStatsRuntime enemyStatsRuntime;

	// Components
	private EnemyBrain brain;
	private EnemyMovement movement;
	private EnemyLongRangeAttack attack;
	private EnemySight sight;
	private EnemyDataManager dataManager;

	public EnemyBrain Brain => brain;
	public EnemyMovement Movement => movement;
	public EnemyLongRangeAttack Attack => attack;
	public EnemySight Sight => sight;
	public EnemyDataManager DataManager => dataManager;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		brain = GetComponent<EnemyBrain>();
		movement = GetComponent<EnemyMovement>();
		attack = GetComponent<EnemyLongRangeAttack>();
		sight = GetComponent<EnemySight>();
		dataManager = GetComponent<EnemyDataManager>();
	}

	private void Start()
	{
		// 플레이어 스탯 초기화
		if (dataManager != null && dataManager._enemyStats != null)
		{
			enemyStatsRuntime = new EnemyStatsRuntime(dataManager._enemyStats);
		}
	}

	public void ChangeState(EnemyState p_state)
	{
		if (brain != null)
		{
			brain.ChangeState(p_state);
		}
	}

	// 데미지 처리
	public void TakeDamage(int attackDamage)
	{
		if (enemyStatsRuntime != null)
		{
			enemyStatsRuntime.CurrentHP -= attackDamage;

			if (enemyStatsRuntime.CurrentHP <= 0)
			{
				OnDeath(attackDamage);
			}
		}
		else
		{
			OnDeath(attackDamage);
		}
	}

	private void OnDeath(int attackDamage)
	{
		if (GameManager.Instance != null && GameManager.Instance.poolManager != null)
		{
			GameManager.Instance.poolManager.ReturnToPool(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		Debug.Log($"적 사망 (피해량: {attackDamage})");
	}
}
