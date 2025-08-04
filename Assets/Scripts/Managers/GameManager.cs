using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// ★ 플레이어 객체 참조용
	public PlayerController player;

	// ★ GameManager 싱글톤 인스턴스
	public static GameManager instance;

	// ※ 스테이지별 플레이어 시작 위치 변수 (추후 구현 예정)
	public Vector3 spawnPosition;

	public static bool isRestart = false;

	// ▶︎ 게임 설정값 참조 (ScriptableObject)
	[SerializeField] private GameSettings gameSettings;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	public void NextStage()
	{
		string sceneName = SceneManager.GetActiveScene().name;

		if (sceneName.StartsWith("Stage_"))
		{
			string indexStr = sceneName.Substring("Stage_".Length);
			if (int.TryParse(indexStr, out int index))
			{
				int nextIndex = index + 1;

				if (nextIndex > gameSettings.maxStageIndex)
				{
					Time.timeScale = 0;
					Debug.Log("게임 클리어!");
					UIManager.instance.ShowRestartButton("Clear!");
					return;
				}

				string nextSceneName = "Stage_" + nextIndex;
				SceneManager.LoadScene(nextSceneName);
			}
		}
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
	public void PlayerReposition()
	{
		player.transform.position = spawnPosition;
		player.VelocityZero();
	}

	// ▶︎ 씬 재시작 처리
	public void Restart()
	{
		isRestart = true;
		PlayerData.instance?.ResetAll(); // 수치만 초기화
		Time.timeScale = 1;
		SceneManager.LoadScene("Stage_0");
	}

	public void SetSpawnPosition(Vector3 pos)
	{
		spawnPosition = pos;
	}
}
