using UnityEngine;

// ▶︎ 플레이어 데이터를 관리하는 클래스
public class PlayerData : MonoBehaviour
{
	// ● 싱글톤 인스턴스
	public static PlayerData instance;

	// ● 현재 체력 및 최대 체력
	public int currentHealth { get; private set; }
	public int maxHealth = 3;

	// ● 현재 스테이지 점수
	public int stageScore { get; private set; }

	// ● 누적 총 점수
	public int totalScore { get; private set; }

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	// ▶︎ 체력 설정
	public void SetHealth(int value)
	{
		currentHealth = Mathf.Clamp(value, 0, maxHealth);
	}

	public void ResetHealth()
	{
		currentHealth = maxHealth;
	}

	// ▶︎ 점수 증가
	public void AddStageScore(int amount)
	{
		stageScore += amount;
		UpdateUIScore();
	}

	// ▶︎ 스테이지 클리어 시 누적 처리
	public void CommitStageScore()
	{
		totalScore += stageScore;
		stageScore = 0;
		UpdateUIScore();
	}

	// ▶︎ 전체 초기화
	public void ResetAll()
	{
		ResetHealth();
		stageScore = 0;
		totalScore = 0;
		UpdateUIScore();
	}

	// ▶︎ UI 표시용 총합 점수
	public int GetTotalDisplayScore()
	{
		return totalScore + stageScore;
	}

	// ▶︎ UI 갱신
	private void UpdateUIScore()
	{
		UIManager.instance?.UpdateScore(GetTotalDisplayScore());
	}
}
