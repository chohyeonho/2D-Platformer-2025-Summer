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

	// ▶︎ 공격 가능한 상태 플래그 (애니메이션 연동용)
	public bool ShouldBeDamaging { get; private set; } = false;

	// ▶︎ 초기화 처리
	private void Start()
	{
		// ✓ 애니메이터 컴포넌트 참조
		anim = GetComponent<Animator>();

		// ✓ 게임 시작 직후 바로 공격 가능하도록 타이머 초기화
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

			// ✓ 애니메이션 트리거 실행
			anim.SetTrigger("attack");

			// ※ 제안: 공격 사운드 재생 추가 고려
			// 
			// audioSource.PlayOneShot(audioAttack);

			// ※ 제안: 여기서 코루틴 직접 시작 가능
			// 
			// StartCoroutine(DamageWhileSlashIsActive());
		}

		// ★ 시간 누적
		attackTimeCounter += Time.deltaTime;
	}

	// ▶︎ 슬래시 애니메이션 재생 중 공격 판정을 지속적으로 감지
	// ● ShouldBeDamaging이 true인 동안 매 프레임 공격 판정 실행
	// ● 애니메이션 이벤트로 On/Off 타이밍 제어 필요
	public IEnumerator DamageWhileSlashIsActive()
	{
		ShouldBeDamaging = true;

		while (ShouldBeDamaging)
		{
			hits = Physics2D.CircleCastAll(
				attackTransform.position,
				attackRange,
				transform.right,
				0f,
				attackableLayer
			);

			for (int i = 0; i < hits.Length; i++)
			{
				IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();

				if (iDamageable != null)
				{
					iDamageable.Damage(damageAmount);
				}
			}

			yield return null;
		}
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
