using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttackSkill", menuName = "Skills/Melee Attack Skill", order = 0)]
public class MeleeAttackSkillData : AttackSkillData
{
	[Header("Melee")]
	public float damage = 1f;
	public float range = 0.5f;
	public GameObject hitEffect;
}
