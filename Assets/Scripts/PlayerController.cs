using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Health.IHealthListener
{
    public float walkingSpeed; // 플레이어 이동속도
    public float jumpSpeed = 5f; // 플레이어 점프속도
    public float mouseSens; // 마우스 감도
    public float verticalSpeed; // 마우스 수직 이동속도
    float horizontalAngle; // 화면 수평 회전각
    float verticalAngle; // 화면 수직 회전각

    public float gravity = 10f; // 중력
    public float terminalSpeed = 20f; // 종단속도

    bool isGrounded; // 공중인가 지상인가 판별할 변수
    float groundTimer; // 떨어져있는 시간을 저장할 변수

    public Transform cameraTransform;
    public List<Weapon> weapons; // 여러 무기를 가지고 있기 위해 List 선언
    int currentWeaponIndex; // 현재 가지고 있는 무기 인덱스
    CharacterController characterController;

    InputAction moveAction;
    InputAction lookAction;
    InputAction fireAction;
    InputAction reloadAction;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        // 커서 관리
        Cursor.lockState = CursorLockMode.Locked; // 커서가 게임 화면을 벗어나지 못하도록
        Cursor.visible = false; // 커서 숨기기

        // New Input System 초기화 (Project Settings - Input System Package)
        InputActionAsset inputActions = GetComponent<PlayerInput>().actions;
        moveAction = inputActions.FindAction("Move"); // Move에 해당하는 입력값 가져오기
        lookAction = inputActions.FindAction("Look"); // Look에 해당하는 입력값 가져오기
        fireAction = inputActions.FindAction("Fire");
        reloadAction = inputActions.FindAction("Reload");
        horizontalAngle = transform.localEulerAngles.y; // 바라보는 방향 가져오기

        // 변수 초기화
        verticalAngle = 0;
        verticalSpeed = 0;
        groundTimer = 0;
        isGrounded = true;
        
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        /// Move
        // 플레이어 이동 구현
        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        // Vector2로 x,y값을 얻었으나 y값은 세로축이므로 y값을 z로 변환해서 사용
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y); 

        // 이동 벡터가 1보다 크면 1로 보정 (대각선 이동은 루트2만큼 이동되므로)
        if (move.magnitude > 1) move.Normalize();

        move = move * walkingSpeed * Time.deltaTime;
        move = transform.TransformDirection(move); // 현재 gameObject의 방향으로 벡터를 돌린다
        characterController.Move(move); // Character Controller에게 이동 명령 전달

        /// Look
        // 마우스 좌우회전
        Vector2 look = lookAction.ReadValue<Vector2>(); // look 액션 벡터를 가져온다
        float turnPlayer = look.x * mouseSens; // 좌우 회전값에 마우스 감도 적용
        horizontalAngle += turnPlayer; // 가로 각도에 회전값을 더함
        if (horizontalAngle >= 360) horizontalAngle -= 360; // 360도 이상일때 360만큼 감소
        if (horizontalAngle < 0) horizontalAngle += 360; // 0도 이하일때 360만큼 증가
        
        Vector3 currentAngle = transform.localEulerAngles; // 변화된 현재 각도를 적용
        currentAngle.y = horizontalAngle;
        transform.localEulerAngles = currentAngle;

        // 마우스 상하회전
        float turnCam = look.y * mouseSens; // 상하 회전값에 마우스 감도 적용
        verticalAngle -= turnCam; // 세로각에 카메라 회전값을 빼줌
                                  // 마우스를 위로 올리면 양수값이 들어오는데 위를 보려면 음수를 넣어줘야해서
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f); // 90도보다 꺾이면 안되니 최대값, 최소값 보정
        currentAngle = cameraTransform.localEulerAngles; // 변화된 현재 각도를 카메라 위치에 전달
        currentAngle.x = verticalAngle; // 시야만 올리기 때문
        cameraTransform.localEulerAngles = currentAngle;

        // 중력
        verticalSpeed -= gravity * Time.deltaTime; // 속도 = 가속도 * 시간

        if (verticalSpeed < -terminalSpeed) // 낙하속도가 종단속도 절대값보다 낮은 값을 가지면
        {
            verticalSpeed = -terminalSpeed; // 낙하속도를 종단속도 절대값으로 고정
        }
        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0); // 낙하벡터 생성
        verticalMove *= Time.deltaTime; // 벡터에 deltaTime 적용

        CollisionFlags flag = characterController.Move(verticalMove); // 캐릭터에게 낙하 적용

        if ((flag & (CollisionFlags.Below | CollisionFlags.Above)) != 0) // 캐릭터의 flag에 Below 비트가 없으면 (떨어지는 상태가 아니면), Above는 점프했을때 머리 위 오브젝트와 부딪혔을때의 처리
        {
            verticalSpeed = 0; // 낙하속도를 0으로 초기화
        }

        // 점프
        /// 땅바닥에 계속 있으면 CollisionFlag Below가 True만 들어와야 하지만
        /// 완벽한 시뮬레이션이 아니기 때문에 땅에 닿고있어도 True와 False가 섞여서 들어온다.
        /// 처리방법 => '일정시간동안 확실하게' 공중에 떨어져 있을 경우 공중 판정을 해준다.
        if (!characterController.isGrounded) // 공중일 경우
        {
            if (isGrounded) // true를 검출하고, 동시에 이전 상태와 비교하기 위해
            {
                groundTimer += Time.deltaTime; // 땅에 닿지않은 시간에 가중치를 주고
                if (groundTimer > 0.3f) // 0.3초를 넘어가면
                {
                    isGrounded = false; // 공중이라고 판단
                }
            }
        }
        else // 지상일 경우
        {
            isGrounded = true; // 지상이라고 판단
            groundTimer = 0; // 가중치 초기화
        }

        // 총
        if (fireAction.WasPressedThisFrame()) // 이번 프레임이 Fire를 했는가?
        {
            weapons[currentWeaponIndex].FireWeapon();
        }
        if (reloadAction.WasPressedThisFrame()) // 이번 프레임이 Fire를 했는가?
        {
            weapons[currentWeaponIndex].ReloadWeapon();
        }
    }

    public void OnChangeWeapon()
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false); // 기존 무기 비활성화

        currentWeaponIndex++; // 가지고 있는 무기의 인덱스를 더함
        if (currentWeaponIndex > weapons.Count - 1) // 인덱스가 범위를 초과할 경우 조정
        {
            currentWeaponIndex = 0;
        }
        weapons[currentWeaponIndex].gameObject.SetActive(true); // 새 무기 활성화
    }

    void OnJump()
    {
        if (isGrounded)
        {
            verticalSpeed = jumpSpeed;
            isGrounded = false;
        }
    }

    public void OnDie()
    {
        GetComponent<Animator>().SetTrigger("Die");
        GameManager.Instance.PlayerDie();
    }
}
