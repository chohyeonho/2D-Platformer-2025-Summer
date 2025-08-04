using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	// ▶︎ 공격 위치 기준 트랜스폼
	[SerializeField] private Transform attackTransform;

	// ▶︎ 공격 대상 설정용 레이어
	[SerializeField] private LayerMask attackableLayer;

	// ▶︎ 플레이어 설정값 참조
	[SerializeField] private PlayerConfig config;

	// ● 데미지를 받은 객체 추적 리스트
	private List<IDamageable> iDamageables = new List<IDamageable>();

	// ● 충돌 판정 결과
	private RaycastHit2D[] hits;

	// ● 애니메이션 제어
	private Animator anim;

	// ● 공격 쿨타임 타이머
	private float attackTimeCounter;

	// ● 공격 유효 구간 여부 (애니메이션 연동)
	public bool ShouldBeDamaging { get; private set; } = false;

	// ● 공격 시 방향 고정용 변수
	private bool cachedFacingLeft = false;

	// ● 플레이어 방향 정보 획득용
	private PlayerController playerController;

	private void Start()
	{
		anim = GetComponent<Animator>();
		playerController = GetComponent<PlayerController>();
		attackTimeCounter = config.attackCooldown;
	}

	private void Update()
	{
		UpdateAttackTransformDirection();

		if (InputManager.instance != null &&
			InputManager.instance.gameInputActions.Player.Attack.WasPressedThisFrame() &&
			attackTimeCounter >= config.attackCooldown)
		{
			attackTimeCounter = 0f;
			anim.SetTrigger("attack");
			GetComponent<PlayerSound>()?.PlaySwing();
		}

		attackTimeCounter += Time.deltaTime;
	}

	private void UpdateAttackTransformDirection()
	{
		if (ShouldBeDamaging)
		{
			Vector3 localPos = attackTransform.localPosition;
			localPos.x = Mathf.Abs(localPos.x) * (cachedFacingLeft ? -1 : 1);
			attackTransform.localPosition = localPos;
			return;
		}

		bool facingLeft = GetComponent<SpriteRenderer>().flipX;
		Vector3 pos = attackTransform.localPosition;
		pos.x = Mathf.Abs(pos.x) * (facingLeft ? -1 : 1);
		attackTransform.localPosition = pos;
	}

	public IEnumerator DamageWhileSlashIsActive()
	{
		ShouldBeDamagingToTrue();
		cachedFacingLeft = GetComponent<SpriteRenderer>().flipX;

		while (ShouldBeDamaging)
		{
			hits = Physics2D.CircleCastAll(
				attackTransform.position,
				config.attackRange,
				transform.right,
				0f,
				attackableLayer
			);

			for (int i = 0; i < hits.Length; i++)
			{
				IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();

				if (iDamageable != null)
				{
					iDamageable.Damaged(config.attackDamage);
					GetComponent<PlayerSound>()?.PlayHit();
					iDamageables.Add(iDamageable);
				}
			}

			yield return null;
		}

		ReturnAttackablesToDamageable();
	}

	private void ReturnAttackablesToDamageable()
	{
		foreach (IDamageable damaged in iDamageables)
		{
			damaged.ResetHitDamageFlag();
		}

		iDamageables.Clear();
	}

	private void OnDrawGizmosSelected()
	{
		if (config == null) return;
		Gizmos.DrawWireSphere(attackTransform.position, config.attackRange);
	}

	public void ShouldBeDamagingToTrue()
	{
		ShouldBeDamaging = true;
	}

	public void ShouldBeDamagingToFalse()
	{
		ShouldBeDamaging = false;
	}
}
