using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackExecutor : MonoBehaviour
{
	[SerializeField] private Transform attackTransform;
	[SerializeField] private LayerMask attackableLayer;

	private readonly List<IDamageable> iDamageables = new List<IDamageable>();
	private RaycastHit2D[] hits;
	private Animator anim;
	private SpriteRenderer spriteRenderer;
	private MeleeAttackSkillData activeSkill;
	private bool cachedFacingLeft;

	public bool ShouldBeDamaging { get; private set; }

	private void Awake()
	{
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		UpdateAttackTransformDirection();
	}

	public void BeginAttack(MeleeAttackSkillData skill)
	{
		if (skill == null) return;

		activeSkill = skill;
		string trigger = string.IsNullOrEmpty(skill.animationTrigger) ? "attack" : skill.animationTrigger;
		anim.SetTrigger(trigger);
		PlayerEvents.OnSwingStarted?.Invoke(this, skill);
	}

	private void UpdateAttackTransformDirection()
	{
		if (attackTransform == null) return;

		if (ShouldBeDamaging)
		{
			Vector3 localPos = attackTransform.localPosition;
			localPos.x = Mathf.Abs(localPos.x) * (cachedFacingLeft ? -1 : 1);
			attackTransform.localPosition = localPos;
			return;
		}

		bool facingLeft = spriteRenderer.flipX;
		Vector3 pos = attackTransform.localPosition;
		pos.x = Mathf.Abs(pos.x) * (facingLeft ? -1 : 1);
		attackTransform.localPosition = pos;
	}

	public IEnumerator DamageWhileSlashIsActive()
	{
		ShouldBeDamagingToTrue();
		cachedFacingLeft = spriteRenderer.flipX;
		iDamageables.Clear();

		while (ShouldBeDamaging)
		{
			if (activeSkill != null)
			{
				hits = Physics2D.CircleCastAll(
					attackTransform.position,
					activeSkill.range,
					transform.right,
					0f,
					attackableLayer
				);

				foreach (var hit in hits)
				{
					if (hit.collider.TryGetComponent(out IDamageable iDamageable) && !iDamageables.Contains(iDamageable))
					{
						iDamageable.Damaged(activeSkill.damage);
						iDamageables.Add(iDamageable);

						if (activeSkill.hitEffect != null)
						{
							Instantiate(activeSkill.hitEffect, hit.point, Quaternion.identity);
						}

						PlayerEvents.OnHitEnemy?.Invoke(this, activeSkill, hit.collider.gameObject);
					}
				}
			}

			yield return null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (attackTransform == null) return;

		float range = activeSkill != null ? activeSkill.range : 0.5f;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackTransform.position, range);
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
