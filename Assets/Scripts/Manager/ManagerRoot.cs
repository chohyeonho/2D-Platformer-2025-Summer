using UnityEngine;

// �� �� ��ȯ �ÿ��� �ı����� �ʵ��� �Ŵ��� �׷� ��ü�� ������Ű�� ��Ʈ ������Ʈ
public class ManagersRoot : MonoBehaviour
{
	// �� ���� �� ����Ǵ� �����ֱ� �Լ�
	private void Awake()
	{
		// �� �� ������Ʈ�� ��� �ڽ� ������Ʈ�� �� �̵� �Ŀ��� ����
		DontDestroyOnLoad(gameObject);
	}
}
