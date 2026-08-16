// PlayerController.cs
using Globals;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rigid;
	private PlayerMovement movement;
	private PlayerAttack attack;
	private PlayerGroundChecker groundChecker;
	private PlayerSlowMode slowMode;
	private PlayerSkillAttack skillAttack;
	private float originalGravity;

	private Collision2D collidedObj;     // 플레이어와 상호작용 할 오브젝트

	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		movement = GetComponent<PlayerMovement>();
		attack = GetComponent<PlayerAttack>();
		slowMode = GetComponent<PlayerSlowMode>();
		groundChecker = GetComponent<PlayerGroundChecker>();
		skillAttack = GetComponent<PlayerSkillAttack>();
	}

	private void Update()
	{
		// 문 상호작용
		if (collidedObj != null)
		{
			if (collidedObj.transform.TryGetComponent(out DoorController door))
			{
				if (movement.inputVec.x != 0)
				{
					print($"Try Open");
					if (door.TryOpen())     // 문 열기 시도해서 성공할 경우 오브젝트 없애기
					{
						collidedObj = null;
					}
				}
			}
		}
	}

	private void Start()
	{
		originalGravity = rigid.gravityScale;
	}

	private void OnMove(InputValue val)
	{
		if (GlobalUtil.IsNullScript(movement)) return;

		Vector2 inputVec = val.Get<Vector2>();

		bool hadNoHorizontal = Mathf.Abs(movement.inputVec.x) < 0.01f;
		bool hasHorizontal = Mathf.Abs(inputVec.x) > 0.01f;

		// 구르기
		if (hadNoHorizontal && hasHorizontal && movement.isCrouchPressed)
		{
			movement.TriggerRollInput();
		}

		movement.inputVec = inputVec;
	}

	private void OnJump(InputValue val)
	{
		if (GlobalUtil.IsNullScript(movement)) return;

		movement.SetJumpInput(val.isPressed);
	}

	private void OnCrouch(InputValue val)
	{
		if (GlobalUtil.IsNullScript(movement)) return;

		if (val.isPressed)
		{
			movement.SetCrouchInput(val.isPressed);
			movement.TriggerRollInput();
		}
		else
		{
			movement.SetCrouchInput(false);
		}
	}

	private void OnAttack(InputValue val)
	{
		if (GlobalUtil.IsNullScript(attack)) return;
		if (val.isPressed)
		{
			rigid.gravityScale = 1f;
			attack.TryAttack();
			rigid.gravityScale = originalGravity;
		}
	}

	private void OnSkillAttack(InputValue val)
	{
		if (val.isPressed)
		{
			skillAttack.EnterSkill();
		}
		else
		{
			skillAttack.ExitSkill();
		}
	}

	private void OnSlow(InputValue val)
	{
		if (GameManager.Instance.playerStatsRuntime.currentHP <= 0)
			return;
		if (val.isPressed)
		{
			// 쿨타임 중이면 아예 슬로우 시도를 막음
			if (slowMode.IsCooldown) return;
			slowMode.EnterSlow();
		}
		else
		{
			// 현재 슬로우 모드가 켜진 상태였을 때만 떼기(종료) 처리 작동
			slowMode.ExitSlow();
		}
	}
}