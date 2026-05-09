using Globals;
using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
	public bool isDashing = false;
	public bool isDashReady = false;

	private Rigidbody2D rigid;
	private Animator animator;
	private PlayerMovement movement;
	private float originalGravity;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		movement = GetComponent<PlayerMovement>();
	}

	public void TryDash()
	{
		if (!isDashReady || isDashing || movement.inputVec == Vector2.zero) return;

		StartCoroutine(DashRoutine());
		isDashReady = false;
	}

	private IEnumerator DashRoutine()
	{
		isDashing = true;
		originalGravity = rigid.gravityScale;
		rigid.gravityScale = 0f;
		animator.Play(PlayerAnimName.slide);

		var stats = GameManager.Instance.playerStatsRuntime;
		float time = 0f;

		while (time < stats.dashDuration)
		{
			rigid.linearVelocity = movement.inputVec.normalized * stats.dashDist;
			time += Time.deltaTime;
			yield return null;
		}

		rigid.gravityScale = originalGravity;
		rigid.linearVelocity = Vector2.zero;
		movement.inputVec = new Vector2(0, movement.inputVec.y);
		animator.Play(PlayerAnimName.idle);
		isDashing = false;
	}
}