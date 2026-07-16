public interface IAttackSkillExecutor
{
	bool CanExecute(AttackSkillData skill);
	bool TryExecute(AttackSkillData skill);
}
