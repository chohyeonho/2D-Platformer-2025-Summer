using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Config/GameSettings")]
public class GameSettings : ScriptableObject
{
	[Header("�ð� ����")]
	public float defaultTimeScale = 1f;

	[Header("���� ����")]
	public float stageScoreMultiplier = 1.0f;

	[Header("UI ��ȯ �ӵ�")]
	public float uiFadeDuration = 0.3f;

	[Header("�ý��� ����")]
	public bool debugMode = false;
	public bool godMode = false;

	[Header("��������")]
	public int maxStageIndex = 3;

	[Header("������ ���� ����")]
	public int bronzeItemScore = 50;
	public int silverItemScore = 100;
	public int goldItemScore = 300;

}
