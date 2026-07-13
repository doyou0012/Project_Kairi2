using UnityEngine;

public class PlayerSkillAttack : MonoBehaviour
{
	[Header("스킬 시전 시 슬로우 비율")]
	[SerializeField] private float slowFactor = 0.2f;
	[Header("플레이어 스킬 사용 최소 거리")]
	[SerializeField] private float skillMinRadius = 1f;
	[Header("플레이어 스킬 사용 최대 거리")]
	[SerializeField] private float skillMaxRadius = 10f;
	[Header("스킬 사용 시 보이는 점")]
	[SerializeField] private GameObject Dot;
	[Header("선 두께")]
	[SerializeField] private float lineWidth = 0.05f;
	private GameObject DotObj;
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
		DotObj.SetActive(false);
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
		DotObj.SetActive(true);
	}

	private void FixedUpdate()
	{
		if (!isActive) return;

		targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
		targetPos.z = DotObj.transform.position.z;

		float dotDist = Vector2.Distance(transform.position, targetPos);
		Vector2 dir = (targetPos - transform.position).normalized;

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

	}

	private void ShowLine()
	{
		if (!DotObj.gameObject.activeSelf)
		{
			DotObj.SetActive(true);
		}
		line.SetPosition(0, transform.position);
		line.SetPosition(1, DotObj.transform.position);
		canUseSkill = true;
	}

	private void HideAll()
	{
		if (DotObj.gameObject.activeSelf)
		{
			DotObj.SetActive(false);
		}
		canUseSkill = false;
	}

	private void SkillAttack()
	{
		transform.position = DotObj.transform.position;
	}

	public void ExitSkill()
	{
		if(canUseSkill) SkillAttack();
		isActive = false;
		line.enabled = false;
		DotObj.SetActive(false);
		slowMode.ExitSlow();

		GetComponent<PlayerMovement>().canMove = true;
		GetComponent<PlayerAttack>().canAttack = true;
	}
}
