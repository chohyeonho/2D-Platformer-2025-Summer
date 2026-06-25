using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Config/GameSettings")]
public class GameSettings : ScriptableObject
{
	[Header("시간 설정")]
	public float defaultTimeScale = 1f;

	[Header("점수 배율")]
	public float stageScoreMultiplier = 1.0f;

	[Header("UI 전환 속도")]
	public float uiFadeDuration = 0.3f;

	[Header("시스템 설정")]
	public bool debugMode = false;
	public bool godMode = false;

	[Header("스테이지")]
	public int maxStageIndex = 3;

	[Header("아이템 점수 설정")]
	public int bronzeItemScore = 50;
	public int silverItemScore = 100;
	public int goldItemScore = 300;

}
