using UnityEngine;

public abstract class AttackSkillData : ScriptableObject
{
	[Header("Info")]
	public string skillId;
	public string displayName;
	public bool learnedByDefault;

	[Header("Usage")]
	public float cooldown = 0.15f;
	public string animationTrigger = "attack";

	[Header("Sounds")]
	public AudioClip swingClip;
	public AudioClip hitClip;
}
