using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
	[SerializeField] private MeleeAttackSkillData basicAttackSkill;

	private IAttackSkillExecutor[] executors;
	private readonly Dictionary<string, float> lastUseTimes = new Dictionary<string, float>();

	private void Awake()
	{
		executors = GetComponents<IAttackSkillExecutor>();
	}

	private void OnEnable()
	{
		PlayerEvents.OnSkillsReset += HandleSkillsReset;
	}

	private void OnDisable()
	{
		PlayerEvents.OnSkillsReset -= HandleSkillsReset;
	}

	private void Start()
	{
		EnsureDefaultSkillsLearned();
	}

	private void Update()
	{
		if (InputManager.instance != null &&
			InputManager.instance.gameInputActions.Player.Attack.WasPressedThisFrame())
		{
			TryUseSkill(basicAttackSkill);
		}
	}

	public bool TryUseSkill(AttackSkillData skill)
	{
		if (!CanUseSkill(skill))
		{
			return false;
		}

		if (executors == null || executors.Length == 0)
		{
			return false;
		}

		foreach (IAttackSkillExecutor executor in executors)
		{
			if (executor == null || !executor.CanExecute(skill))
			{
				continue;
			}

			if (executor.TryExecute(skill))
			{
				lastUseTimes[skill.skillId] = Time.time;
				return true;
			}
		}

		return false;
	}

	public void LearnSkill(string skillId)
	{
		if (PlayerData.instance == null || string.IsNullOrEmpty(skillId)) return;
		PlayerData.instance.LearnSkill(skillId);
	}

	public bool IsSkillLearned(string skillId)
	{
		return PlayerData.instance != null && PlayerData.instance.IsSkillLearned(skillId);
	}

	public bool CanUseSkill(AttackSkillData skill)
	{
		if (skill == null || string.IsNullOrEmpty(skill.skillId))
		{
			return false;
		}

		if (!IsSkillLearned(skill.skillId))
		{
			return false;
		}

		if (lastUseTimes.TryGetValue(skill.skillId, out float lastUseTime) &&
			Time.time < lastUseTime + skill.cooldown)
		{
			return false;
		}

		return true;
	}

	private void HandleSkillsReset(object sender)
	{
		lastUseTimes.Clear();
		EnsureDefaultSkillsLearned();
	}

	private void EnsureDefaultSkillsLearned()
	{
		if (PlayerData.instance == null) return;

		if (basicAttackSkill != null &&
			basicAttackSkill.learnedByDefault &&
			!string.IsNullOrEmpty(basicAttackSkill.skillId))
		{
			PlayerData.instance.LearnSkill(basicAttackSkill.skillId);
		}
	}
}
