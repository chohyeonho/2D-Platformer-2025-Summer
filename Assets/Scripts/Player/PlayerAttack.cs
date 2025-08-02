using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	// ★ 공격 위치 기준 트랜스폼
	[SerializeField] private Transform attackTransform;

	// ★ 공격 범위 반지름
	[SerializeField] private float attackRange = 1.5f;

	// ★ 공격 가능 대상 레이어
	[SerializeField] private LayerMask attackableLayer;

	// ★ 공격 데미지 수치
	[SerializeField] private float damageAmount = 1f;

	// ★ 공격 간격 시간 (초)
	[SerializeField] private float timeBtwAttacks = 0.15f;

	// ● 이번 공격 중 데미지를 받은 객체 목록
	private List<IDamageable> iDamageables = new List<IDamageable>();

	// ● 공격 판정 결과 저장용 배열
	private RaycastHit2D[] hits;

	// ● 애니메이션 제어용
	private Animator anim;

	// ● 공격 시간 누적용 타이머
	private float attackTimeCounter;

	// ▶︎ 공격 가능한 상태 플래그 (애니메이션 연동용)
	public bool ShouldBeDamaging { get; private set; } = false;

	// ● 공격 중 방향 고정을 위한 변수
	private bool cachedFacingLeft = false;

	// ● 플레이어 이동 스크립트 참조
	private PlayerController playerController;


	// ▶︎ 초기화 처리
	private void Start()
	{
		// ✓ 애니메이터 컴포넌트 참조
		anim = GetComponent<Animator>();

		playerController = GetComponent<PlayerController>();

		// ✓ 게임 시작 직후 바로 공격 가능하도록 타이머 초기화
		attackTimeCounter = timeBtwAttacks;
	}

	// ▶︎ 키 입력 감지
	private void Update()
	{
		// ✓ 공격 위치 좌우 반영
		UpdateAttackTransformDirection();

		// ✓ 공격 키를 눌렀는지 확인 + 쿨타임 체크
		if (InputManager.instance != null &&
			InputManager.instance.gameInputActions.Player.Attack.WasPressedThisFrame() &&
			attackTimeCounter >= timeBtwAttacks)
		{
			attackTimeCounter = 0f;

			anim.SetTrigger("attack");

			// ★ 휘두를 때 사운드
			GetComponent<PlayerSound>()?.PlaySwing();
		}

		// ★ 시간 누적
		attackTimeCounter += Time.deltaTime;
	}

	// ▶︎ 플레이어의 방향에 따라 공격 위치를 좌우 반전
	private void UpdateAttackTransformDirection()
	{
		// ✓ 공격 중이면 고정된 방향 유지
		if (ShouldBeDamaging)
		{
			Vector3 localPos = attackTransform.localPosition;
			localPos.x = Mathf.Abs(localPos.x) * (cachedFacingLeft ? -1 : 1);
			attackTransform.localPosition = localPos;
			return;
		}

		// ✓ 평상시엔 현재 방향 기준으로 계속 갱신
		bool facingLeft = GetComponent<SpriteRenderer>().flipX;
		Vector3 pos = attackTransform.localPosition;
		pos.x = Mathf.Abs(pos.x) * (facingLeft ? -1 : 1);
		attackTransform.localPosition = pos;
	}


	// ▶︎ 슬래시 애니메이션 재생 중 공격 판정을 지속적으로 감지
	// ● ShouldBeDamaging이 true인 동안 매 프레임 공격 판정 실행
	// ● 애니메이션 이벤트로 On/Off 타이밍 제어 필요
	public IEnumerator DamageWhileSlashIsActive()
	{
		// ★ 데미지 가능 상태로 설정
		ShouldBeDamagingToTrue();

		// ★ 방향 고정 시작
		cachedFacingLeft = GetComponent<SpriteRenderer>().flipX;

		while (ShouldBeDamaging)
		{
			// ★ 공격 범위 내 모든 충돌 대상 탐지
			hits = Physics2D.CircleCastAll(
				attackTransform.position,
				attackRange,
				transform.right,
				0f,
				attackableLayer
			);

			for (int i = 0; i < hits.Length; i++)
			{
				// ★ 인터페이스 구현체 가져오기
				IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();

				// ✓ 아직 데미지를 받지 않은 대상일 경우
				if (iDamageable != null && !iDamageable.HasTakenDamage)
				{
					iDamageable.Damage(damageAmount);

					// ★ 적중 사운드 재생
					GetComponent<PlayerSound>()?.PlayHit();

					iDamageables.Add(iDamageable);
				}
			}

			// ★ 다음 프레임까지 대기
			yield return null;
		}

		// ★ 피격 상태 초기화
		ReturnAttackablesToDamageable();
	}

	// ▶︎ 데미지를 받은 IDamageable 목록을 초기화하는 함수
	private void ReturnAttackablesToDamageable()
	{
		// ★ 데미지 받은 객체 목록 반복
		foreach (IDamageable thingThatWasDamaged in iDamageables)
		{
			// ★ 중복 방지를 위해 피격 상태 초기화
			thingThatWasDamaged.HasTakenDamage = false;
		}

		// ★ 다음 공격을 위해 리스트 초기화
		iDamageables.Clear();
	}

	// ▶︎ 공격 범위 디버그용 기즈모 표시
	private void OnDrawGizmosSelected()
	{
		// ✓ 에디터에서 공격 범위 시각화
		Gizmos.DrawWireSphere(attackTransform.position, attackRange);
	}

	// ▶︎ 애니메이션 이벤트에서 호출하여 데미지 유효 구간 시작
	public void ShouldBeDamagingToTrue()
	{
		ShouldBeDamaging = true;
	}

	// ▶︎ 애니메이션 이벤트에서 호출하여 데미지 유효 구간 종료
	public void ShouldBeDamagingToFalse()
	{
		ShouldBeDamaging = false;
	}

	// ※ (비활성화됨) 단발 공격 처리 함수 - 현재 사용되지 않음
	//private void Attack()
	//{
	//	hits = Physics2D.CircleCastAll(
	//		attackTransform.position,
	//		attackRange,
	//		transform.right,
	//		0f,
	//		attackableLayer
	//	);

	//	for (int i = 0; i < hits.Length; i++)
	//	{
	//		IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();
	//		if (iDamageable != null)
	//		{
	//			iDamageable.Damage(damageAmount);
	//		}
	//	}
	//}

	// ※ 제안: 추후 필요 시 공격 쿨타임 변수 도입 고려
	// 
	// private float attackCooldown = 0.5f;

	// ※ 제안: 공격 범위를 원이 아닌 BoxCast로 설정 가능
	// 
	// Physics2D.BoxCast(...);

	// ※ 제안: 태그별 공격 반응 분기 처리 고려
	// 
	// if (hits[i].collider.CompareTag("Enemy")) { ... }
}