using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// ★ 전체 점수 누적용 변수
	public int totalPoint;

	// ★ 현재 스테이지에서 획득한 점수
	public int stagePoint;

	// ★ 현재 스테이지 인덱스 (예: 0, 1, 2…)
	public int stageIndex;

	// ★ 스테이지 배열
	public GameObject[] Stages;

	// ★ 플레이어 객체 참조용
	public PlayerController player;

	// ★ GameManager 싱글톤 인스턴스
	public static GameManager instance;

	// ※ 스테이지별 플레이어 시작 위치 변수 (추후 구현 예정)
	public Vector3 spawnPosition;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	void Update()
	{
		// ★ 점수 UI 실시간 갱신
		UIManager.instance.UpdateScore(totalPoint + stagePoint);
	}

	// ▶︎ 다음 스테이지로 전환 처리
	public void NextStage()
	{
		if (stageIndex < Stages.Length - 1)
		{
			Stages[stageIndex].SetActive(false);
			stageIndex++;
			Stages[stageIndex].SetActive(true);
			PlayerReposition();
			UIManager.instance.UpdateStage(stageIndex);
		}
		else
		{
			Time.timeScale = 0;
			Debug.Log("게임 클리어!");
			UIManager.instance.ShowRestartButton("Clear!");
		}

		totalPoint += stagePoint;
		stagePoint = 0;
	}

	// ▶︎ 플레이어 충돌 시 체력 조건에 따른 처리
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayerHealth hp = player.GetComponent<PlayerHealth>();

			if (hp != null)
			{
				if (hp.GetCurrentHealth() > 1)
				{
					PlayerReposition();
				}

				hp.TakeDamage(player.transform.position);
			}
		}
	}

	// ▶︎ 플레이어 위치 초기화
	void PlayerReposition()
	{
		player.transform.position = spawnPosition;
		player.VelocityZero();
	}

	// ▶︎ 씬 재시작 처리
	public void Restart()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
