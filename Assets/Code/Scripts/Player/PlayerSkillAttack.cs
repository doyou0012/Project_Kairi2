using Globals;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerSkillAttack : MonoBehaviour
{
	[Header("스킬 시전 시 슬로우 비율")]
	[SerializeField] private float slowFactor = 0.01f;
	[Header("플레이어 스킬 사용 최소 거리")]
	[SerializeField] private float skillMinRadius = 1f;
	[Header("플레이어 스킬 사용 최대 거리")]
	[SerializeField] private float skillMaxRadius = 10.4f;
	[Header("스킬 사용 시 보이는 점")]
	[SerializeField] private GameObject Dot;
	[Header("스킬 사용 시 보이는 선")]
	[SerializeField] private GameObject LimitLine;
	[Header("선 두께")]
	[SerializeField] private float lineWidth = 0.05f;
	private GameObject DotObj;
	private GameObject LineObj;
	private LineRenderer line;
	private PlayerSlowMode slowMode;
	private Camera mainCam;
	private Vector3 targetPos;
	public bool isActive = false;
	public bool canUseSkill = true;

	private void Awake()
	{
		slowMode = GetComponent<PlayerSlowMode>();
	}

	private void Start()
	{
		mainCam = Camera.main;
		DotObj = Instantiate(Dot);
		DotObj.SetActive(false);	// 점 안 보이게
		LineObj = Instantiate(LimitLine);
		LineObj.SetActive(false);	// 선 안 보이게
		SetLine();
	}

	private void SetLine()
	{
		line = DotObj.GetComponentInChildren<LineRenderer>();
		line.positionCount = 2;
		line.widthMultiplier = lineWidth;
		line.startColor = Color.white;
		line.endColor = Color.white;
	}

	public void EnterSkill()
	{
		GetComponent<PlayerMovement>().canMove = false;		// 움직임 제한
		GetComponent<PlayerAttack>().canAttack = false;		// 공격 제한

		if (Dot == null)
		{
			Debug.LogWarning("Dot 오브젝트 없음");
			return;
		}
		isActive = true;
		line.enabled = true;
		slowMode.EnterSlow(slowFactor);
		SetActiveObj(true);
	}

	private void FixedUpdate()
	{
		if (!isActive) return;

		targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
		targetPos.z = DotObj.transform.position.z;

		float dotDist = Vector2.Distance(transform.position, targetPos);
		Vector2 dir = (targetPos - transform.position).normalized;

		// 점 오브젝트 설정
		if (dotDist > skillMaxRadius)
		{
			DotObj.transform.position = transform.position + (Vector3)(dir * skillMaxRadius);
			ShowLine();     // 선 보이기
		}
		else if (dotDist < skillMinRadius)
		{
			DotObj.transform.position = transform.position + (Vector3)(dir * skillMinRadius);
			HideAll();	// 전체 효과 숨기기
		}
		else
		{
			DotObj.transform.position = targetPos;
			ShowLine();     // 선 보이기
		}

		// 선 오브젝트 설정
		if(LineObj.transform.position != transform.position)
			LineObj.transform.position = transform.position;
	}

	private void ShowLine()
	{
		if (!DotObj.gameObject.activeSelf)
		{
			SetActiveObj(true);
		}
		line.SetPosition(0, transform.position);
		line.SetPosition(1, DotObj.transform.position);
		canUseSkill = true;
	}

	private void HideAll()
	{
		if (DotObj.gameObject.activeSelf)
		{
			SetActiveObj(false);
		}
		canUseSkill = false;
	}

	private void SetActiveObj(bool active)
	{
		DotObj.SetActive(active);
		LineObj.SetActive(active);
	}

	// 스킬 사용
	private void SkillAttack()
	{
		Vector2 targetPos = DotObj.transform.position;		// 이동할 위치
		Vector2 boxSize = Vector2.Scale(GetComponent<BoxCollider2D>().size, transform.lossyScale);
		Vector2 dir = ((Vector3)targetPos - transform.position).normalized;
		LayerMask mask = LayerMask.GetMask(LayerName.ground, TagName.wall);
		RaycastHit2D hit = Physics2D.BoxCast(
			transform.position,
			boxSize,
			transform.eulerAngles.z,
			dir,
			skillMaxRadius,
			mask
		);

		if (hit)
		{
			targetPos = hit.point;
		}

		AttackHitMobs(targetPos);
		transform.position = targetPos;
	}

	private void AttackHitMobs(Vector2 target)
	{
		LayerMask mask = LayerMask.GetMask(LayerName.enemy, LayerName.crackObj);

		Vector2 dir = (target - (Vector2)transform.position).normalized;
		float distance = Vector2.Distance(transform.position, target);

		RaycastHit2D[] hits = Physics2D.RaycastAll(
			transform.position,
			dir,
			distance,
			mask
		);

		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform.TryGetComponent<IDamageable>(out var dm))
			{
				dm.TakeDamage(GameManager.Instance.playerStatsRuntime.attack);
			}
		}
	}

	// 마우스 뗌과 동시에 스킬 나가기 및 사용
	public void ExitSkill()
	{
		if(canUseSkill) SkillAttack();
		isActive = false;
		line.enabled = false;
		SetActiveObj(false);
		slowMode.ExitSlow();

		GetComponent<PlayerMovement>().canMove = true;
		GetComponent<PlayerAttack>().canAttack = true;
	}
}
