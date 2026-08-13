using UnityEngine;

/// <summary>
/// 글로벌 변수를 관리하는 파일
/// **반드시 불변하는 값(읽기전용)만 지정할 것!!!**
/// </summary>
namespace Globals
{
	// 태그 이름(string) 관련 클래스
	public static class TagName
	{
		// 적
		public const string enemy = "Enemy";
		public const string bullet = "Bullet";
		// 오브젝트
		public const string obj = "NormalObject";
		public const string crackObj = "CrackObject";
		public const string expObj = "ExplosionObject";
		public const string door = "Door";
		// 플레이어 관련
		public const string player = "Player";
		public const string hook = "Hook";
		// 배경 요소
		public const string ground = "Ground";
		public const string wall = "Wall";
		public const string oneWayPlatform = "OneWayPlatform";
		public const string trigger = "Trigger";
		// NPC
		public const string npc = "NPC";
		// 카메라
		public const string cameraBound = "CameraBound";
	}

	public static class LayerName
	{
		public const string ground = "Ground";
		public const string oneWayPlatform = "OneWayPlatform";
		public const string crackObj = "CrackObject";
		public const string player = "Player";
		public const string enemy = "Enemy";
		public const string wall = "Wall";
	}

	// 애니메이션 이름 관련 클래스
	public static class EnemyAnimName	// 적
	{
		public const string idle = "Enemy_Idle";
		public const string chase = "Enemy_Run";
		public const string attack = "Enemy_Shot1";
		public const string patrol = "Enemy_Walk";
		public const string recharge = "Enemy_Recharge";
	}
	public static class PlayerAnimName   // 플레이어
	{
		public const string idle		= "Dragon_Idle";
		public const string run			= "Dragon_Run";
		public const string jump		= "Dragon_Jump";
		public const string attack		= "Dragon_Attack";
		public const string down		= "Dragon_Down";
		public const string landing		= "Dragon_Landing";
		public const string landUp		= "Dragon_LandUp";
		public const string slide		= "Dragon_Slide";
		public const string roll		= "Dragon_Roll";
		public const string redgeClimb	= "Dragon_RedgeClimb";
		public const string climb		= "Dragon_Climb";
		public const string climbSlide	= "Dragon_ClimbSlide";
	}

	// 프리펩 이름 관련 클래스
	public static class PrefabName
	{
		public const string bullet = "Bullet";
	}

	///  - 유니티의 애니메이터(Animator) 엔진은 문자열(String)로 글자를 정확하게 불러줘야만 애니메이션을 틀어줍니다.
	///  - 하지만 개발자가 여러 스크립트에 일일이 손으로 "Enemy_Idle"이라고 적다 보면 오타("Enemy_idel" 등)를 내서 모션이 굳어버리는 버그가 아주 흔하게 발생합니다.
	///  - 이를 방지하기 위해, 오타 걱정 없는 정답 글자를 이 상자 안에 딱 한 번만 적어두고, 다른 스크립트에서 자동 완성(KimEnemyAnimName.idle)으로 꺼내 쓸 수 있도록 만듭니다!
	public static class KimEnemyAnimName
	{
		// 🔍 [C# 키워드 정밀 분석]
		// 💡 유니티 애니메이터 컨트롤러 안에 만들어진 진짜 "Idle" 상태 노드의 스펠링과 100% 동일하게 글자를 맞춰 적어둡니다.
		public const string idle = "Enemy_Idle";
		// 💡 유니티 애니메이터 컨트롤러 안에 만들어진 순찰 걷기("Walk") 노드 스펠링과 일치시킵니다.
		public const string patrol = "Enemy_Walk";
		// 💡 유니티 애니메이터 컨트롤러 안에 만들어진 공격("Shot1") 노드 스펠링과 일치시킵니다.
		public const string attack = "Enemy_Shot1";
		// 💡 유니티 애니메이터 컨트롤러 안에 만들어진 맹렬한 질주("Run") 노드 스펠링과 일치시킵니다.
		public const string chase = "Enemy_Run";
		// 💀 [신규 추가] 적이 으악 하고 쓰러지는 실제 유니티 죽음 애니메이션 노드 이름과 일치시킵니다.
		public const string dead = "Enemy_Die";
	}








}