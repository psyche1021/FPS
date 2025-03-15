using UnityEngine;

public class LookCamera : MonoBehaviour
{
    void Update()
    {
        // 게이지 바를 카메라와 평행하게 조절, 어느 방향에서든 적의 HP가 보이도록
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
