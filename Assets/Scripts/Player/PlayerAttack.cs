using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	// ▶︎ 공격 위치 기준 트랜스폼
	[SerializeField] private Transform attackTransform;

	// ▶︎ 공격 대상 설정용 레이어
	[SerializeField] private LayerMask attackableLayer;

	// ▶︎ 무기 매니저 참조
	private WeaponController weaponController;

	// ● 현재 무기의 데이터 캐싱
	private WeaponData currentWeapon;

	// ● 데미지를 받은 객체 추적 리스트
	private readonly List<IDamageable> iDamageables = new List<IDamageable>();

	// ● 충돌 판정 결과
	private RaycastHit2D[] hits;

	// ● 컴포넌트 캐싱
	private Animator anim;

	// ● 공격 쿨타임 타이머
	private float lastAttackTime;

	// ● 공격 유효 구간 여부 (애니메이션 연동)
	public bool ShouldBeDamaging { get; private set; } = false;

	// ● 공격 시 방향 고정용 변수
	private bool cachedFacingLeft = false;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		weaponController = GetComponent<WeaponController>();
	}

	private void Start()
	{
		WeaponData weapon = weaponController.GetCurrentWeapon();
		
		// 현재 무기가 없으면 기본 칼 사용
		if (weapon == null)
		{
			weapon = Resources.Load<WeaponData>("Configs/Weapons/Unarmed");
			if (weapon == null)
			{
				Debug.LogError("기본 무기(Unarmed)를 Resources/Configs/Weapons 폴더에서 찾을 수 없습니다!");
				return;
			}
		}
		
		UpdateWeapon(weapon);
	}

	private void Update()
	{
		UpdateAttackTransformDirection();

		if (InputManager.instance != null && currentWeapon != null &&
			InputManager.instance.gameInputActions.Player.Attack.WasPressedThisFrame() &&
			Time.time >= lastAttackTime + currentWeapon.attackDelay)
		{
			Attack();
		}
	}

	private void Attack()
	{
		lastAttackTime = Time.time;

		anim.SetTrigger("attack");

		PlayerEvents.OnSwingStarted?.Invoke(this);
	}
	public void UpdateWeapon(WeaponData newWeapon)
	{
		if (newWeapon == null)
		{
			Debug.LogWarning("UpdateWeapon: newWeapon이 null입니다!");
			return;
		}
		
		currentWeapon = newWeapon;
		lastAttackTime = -currentWeapon.attackDelay; // 즉시 공격 가능하도록 설정
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

		iDamageables.Clear();

		while (ShouldBeDamaging)
		{
			hits = Physics2D.CircleCastAll(
				attackTransform.position,
				currentWeapon.range,
				transform.right,
				0f,
				attackableLayer
			);

			foreach (var hit in hits)
			{
				if (hit.collider.TryGetComponent(out IDamageable iDamageable) && !iDamageables.Contains(iDamageable))
				{
					iDamageable.Damaged(currentWeapon.damage);
					iDamageables.Add(iDamageable);

					if (currentWeapon.hitEffect != null)
					{
						Instantiate(currentWeapon.hitEffect, hit.point, Quaternion.identity);
					}

					PlayerEvents.OnHitEnemy?.Invoke(this, hit.collider.gameObject);
				}
			}

			yield return null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (currentWeapon == null) return;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackTransform.position, currentWeapon.range);
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
