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

	// ● 공격 판정 결과 저장용 배열
	private RaycastHit2D[] hits;

	// ● 애니메이션 제어용
	private Animator anim;

	// ▶︎ 초기화 처리
	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	// ▶︎ 키 입력 감지
	private void Update()
	{
		// ✓ 공격 키를 눌렀는지 확인
		if (UserInput.instance != null && UserInput.instance.controls.Player.Attack.WasPressedThisFrame())
		{
			// ✓ 공격 함수 호출
			Attack();

			// ✓ 공격 애니메이션 재생
			anim.SetTrigger("attack");

			// ※ 제안: 공격 사운드 재생
			// audioSource.PlayOneShot(audioAttack);
		}
	}

	// ▶︎ 공격 처리
	private void Attack()
	{
		// ✓ 원형 범위 내 충돌 감지
		hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

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
		Gizmos.DrawWireSphere(attackTransform.position, attackRange);
	}

	// ※ 제안: 추후 필요 시 공격 쿨타임 변수 도입 고려
	// private float attackCooldown = 0.5f;

	// ※ 제안: 공격 범위 직선이 아닌 BoxCast도 고려 가능
	// Physics2D.BoxCast(...);

	// ※ 제안: 공격 대상 태그 확인 후 전용 반응 처리 가능
	// if (hits[i].collider.CompareTag("Enemy")) { ... }
}
