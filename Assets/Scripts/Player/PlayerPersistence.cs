using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
	// ● 싱글 인스턴스 참조 (중복 생성을 막기 위한 감시자 용도)
	private static PlayerPersistence instance;

	// ▶︎ 초기화 처리
	private void Awake()
	{
		// ★ 이미 인스턴스가 있다면 자신은 즉시 파괴
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		// ★ 최초 인스턴스 설정
		instance = this;

		// ★ 씬 이동 시에도 파괴되지 않도록 설정
		DontDestroyOnLoad(gameObject);
	}
}
