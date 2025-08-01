using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ▶︎ 플레이어 근접 공격 처리 클래스
public class PlayerAttack : MonoBehaviour
{
	// ★ 공격 판정 기준 위치 (검 끝 위치 등)
	[SerializeField] private Transform attackTransform;

	// ★ 공격 범위 (원형 판정 반지름)
	[SerializeField] private float attackRange = 1.5f;

	// ★ 공격 가능한 대상 레이어 마스크
	[SerializeField] private LayerMask attackableLayer;

	// ● 공격 판정 결과 저장용 배열
	private RaycastHit2D[] hits;

	// ▶︎ 키 입력 감지
	private void Update()
	{
		// ✓ 공격 키를 눌렀는지 확인
		if (UserInput.instance.controls.Player.Attack.WasPressedThisFrame())
		{
			//attack

			// ※ 제안: 공격 함수 호출
			// Attack();

			// ※ 제안: 공격 애니메이션 재생
			// anim.SetTrigger("doAttack");

			// ※ 제안: 공격 사운드 재생
			// audioSource.PlayOneShot(audioAttack);
		}
	}

	// ▶︎ 공격 판정 처리
	private void Attack()
	{
		// ✓ 공격 기준 위치에서 원형으로 충돌 판정
		hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

		// ✓ 충돌된 대상들을 하나씩 확인
		for (int i = 0; i < hits.Length; i++)
		{
			// ✓ EnemyHealth 컴포넌트를 가진 적 찾기
			EnemyHealth enemyHealth = hits[i].collider.gameObject.GetComponent<EnemyHealth>();

			// ✓ 적이 맞다면 데미지 처리
			if (enemyHealth != null)
			{
				//damage

				// ※ 제안: EnemyHealth에 데미지 함수 호출
				// enemyHealth.OnDamaged();

				// ※ 제안: 이펙트 생성 및 연출
				// Instantiate(hitEffect, hits[i].point, Quaternion.identity);
			}
		}
	}

	// ※ 제안: 컴포넌트 캐싱용 변수들 (필요 시 사용)
	// private Animator anim;
	// private AudioSource audioSource;
	// public AudioClip audioAttack;
	// public GameObject hitEffect;

	// ※ 제안: Awake에서 컴포넌트 연결
	// private void Awake()
	// {
	//     anim = GetComponent<Animator>();
	//     audioSource = GetComponent<AudioSource>();
	// }
}
