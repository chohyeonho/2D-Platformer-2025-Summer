using UnityEngine;

// ● 씬 전환 시에도 파괴되지 않도록 매니저 그룹 전체를 유지시키는 루트 오브젝트
public class ManagersRoot : MonoBehaviour
{
	// ● 시작 시 실행되는 생명주기 함수
	private void Awake()
	{
		// ● 이 오브젝트와 모든 자식 오브젝트를 씬 이동 후에도 유지
		DontDestroyOnLoad(gameObject);
	}
}
