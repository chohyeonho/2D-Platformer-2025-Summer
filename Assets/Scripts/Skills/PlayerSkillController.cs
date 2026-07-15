using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
	[SerializeField] private List<AttackSkillData> skillSlots = new List<AttackSkillData>();
	[SerializeField] private List<AttackSkillData> skillCatalog = new List<AttackSkillData>();

	private MeleeAttackExecutor meleeAttackExecutor;
	private readonly Dictionary<string, float> lastUseTimes = new Dictionary<string, float>();

	private void Awake()
	{
		meleeAttackExecutor = GetComponent<MeleeAttackExecutor>();
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
			TryUseSlot(0);
		}
	}

	public bool TryUseSlot(int slotIndex)
	{
		AttackSkillData skill = GetSkillInSlot(slotIndex);
		if (!CanUseSkill(skill))
		{
			return false;
		}

		ExecuteSkill(skill);
		return true;
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

	public void SetSkillSlot(int slotIndex, AttackSkillData skill)
	{
		if (slotIndex < 0) return;

		while (skillSlots.Count <= slotIndex)
		{
			skillSlots.Add(null);
		}

		skillSlots[slotIndex] = skill;
	}

	private AttackSkillData GetSkillInSlot(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= skillSlots.Count)
		{
			return null;
		}

		return skillSlots[slotIndex];
	}

	private void ExecuteSkill(AttackSkillData skill)
	{
		lastUseTimes[skill.skillId] = Time.time;

		if (skill is MeleeAttackSkillData meleeSkill)
		{
			if (meleeAttackExecutor == null)
			{
				Debug.LogWarning("MeleeAttackExecutor가 없어 근거리 스킬을 실행할 수 없습니다.");
				return;
			}

			meleeAttackExecutor.BeginAttack(meleeSkill);
			return;
		}

		Debug.LogWarning($"지원하지 않는 공격 스킬 타입입니다: {skill.GetType().Name}");
	}

	private void HandleSkillsReset(object sender)
	{
		lastUseTimes.Clear();
		EnsureDefaultSkillsLearned();
	}

	private void EnsureDefaultSkillsLearned()
	{
		if (PlayerData.instance == null) return;

		foreach (AttackSkillData skill in EnumerateKnownSkills())
		{
			if (skill != null && skill.learnedByDefault && !string.IsNullOrEmpty(skill.skillId))
			{
				PlayerData.instance.LearnSkill(skill.skillId);
			}
		}
	}

	private IEnumerable<AttackSkillData> EnumerateKnownSkills()
	{
		var seen = new HashSet<AttackSkillData>();

		foreach (AttackSkillData skill in skillSlots)
		{
			if (skill != null && seen.Add(skill))
			{
				yield return skill;
			}
		}

		foreach (AttackSkillData skill in skillCatalog)
		{
			if (skill != null && seen.Add(skill))
			{
				yield return skill;
			}
		}
	}
}
