using UnityEngine;

public class PlayerData : MonoBehaviour
{
	// ● 싱글톤 인스턴스
	public static PlayerData instance;

	// ▶︎ 플레이어 설정값 참조
	[SerializeField] private PlayerConfig config;

	// ● 현재 체력
	public int currentHealth { get; private set; }

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

		ResetAll();
	}

	// ▶︎ 체력 설정
	public void SetHealth(int value)
	{
		currentHealth = Mathf.Clamp(value, 0, config.maxHealth);
	}

	// ▶︎ 체력 초기화
	public void ResetHealth()
	{
		currentHealth = config.maxHealth;
	}

	// ▶︎ 점수 증가
	public void AddStageScore(int amount)
	{
		stageScore += amount;

		// ● 점수 변경 이벤트 발생
		PlayerEvents.OnScoreChanged?.Invoke(this, GetTotalDisplayScore());
	}

	// ▶︎ 스테이지 클리어 시 누적 처리
	public void CommitStageScore()
	{
		totalScore += stageScore;
		stageScore = 0;

		// ● 점수 변경 이벤트 발생
		PlayerEvents.OnScoreChanged?.Invoke(this, GetTotalDisplayScore());
	}

	// ▶︎ 전체 초기화
	public void ResetAll()
	{
		ResetHealth();
		stageScore = 0;
		totalScore = 0;

		// ● 점수 변경 이벤트 발생
		PlayerEvents.OnScoreChanged?.Invoke(this, GetTotalDisplayScore());
	}

	// ▶︎ UI 표시용 총합 점수
	public int GetTotalDisplayScore()
	{
		return totalScore + stageScore;
	}
}
