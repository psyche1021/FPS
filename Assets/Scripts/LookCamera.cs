using UnityEngine;

public class LookCamera : MonoBehaviour
{
    void Update()
    {
        // ������ �ٸ� ī�޶�� �����ϰ� ����, ��� ���⿡���� ���� HP�� ���̵���
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
