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

	// ● 공격 판정 결과 저장용 배열
	private RaycastHit2D[] hits;

	// ● 애니메이션 제어용
	private Animator anim;

	// ● 공격 시간 누적용 타이머
	private float attackTimeCounter;

	// ▶︎ 초기화 처리
	private void Start()
	{
		// ✓ 애니메이터 컴포넌트 참조
		anim = GetComponent<Animator>();

		attackTimeCounter = timeBtwAttacks;
	}

	// ▶︎ 키 입력 감지
	private void Update()
	{
		// ✓ 공격 키를 눌렀는지 확인 + 쿨타임 체크
		if (UserInput.instance != null &&
			UserInput.instance.controls.Player.Attack.WasPressedThisFrame() &&
			attackTimeCounter >= timeBtwAttacks)
		{
			// ★ 타이머 초기화
			attackTimeCounter = 0f;

			// ✓ 공격 함수 호출
			Attack();

			// ✓ 공격 애니메이션 재생
			anim.SetTrigger("attack");

			// ※ 제안: 공격 사운드 재생 추가 고려
			// 
			// audioSource.PlayOneShot(audioAttack);
		}

		// ★ 시간 누적
		attackTimeCounter += Time.deltaTime;
	}

	// ▶︎ 공격 처리
	private void Attack()
	{
		// ✓ 원형 범위 내 충돌 감지
		hits = Physics2D.CircleCastAll(
			attackTransform.position,
			attackRange,
			transform.right,
			0f,
			attackableLayer
		);

		for (int i = 0; i < hits.Length; i++)
		{
			// ✓ IDamageable 인터페이스 구현체 탐색
			IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();

			// ✓ 공격 가능 대상일 경우 데미지 적용
			if (iDamageable != null)
			{
				iDamageable.Damage(damageAmount);
			}
		}
	}

	// ▶︎ 공격 범위 디버그용 기즈모 표시
	private void OnDrawGizmosSelected()
	{
		// ✓ 에디터에서 공격 범위 시각화
		Gizmos.DrawWireSphere(attackTransform.position, attackRange);
	}

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
