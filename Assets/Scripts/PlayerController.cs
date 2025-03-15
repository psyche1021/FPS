using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Health.IHealthListener
{
    public float walkingSpeed; // �÷��̾� �̵��ӵ�
    public float jumpSpeed = 5f; // �÷��̾� �����ӵ�
    public float mouseSens; // ���콺 ����
    public float verticalSpeed; // ���콺 ���� �̵��ӵ�
    float horizontalAngle; // ȭ�� ���� ȸ����
    float verticalAngle; // ȭ�� ���� ȸ����

    public float gravity = 10f; // �߷�
    public float terminalSpeed = 20f; // ���ܼӵ�

    bool isGrounded; // �����ΰ� �����ΰ� �Ǻ��� ����
    float groundTimer; // �������ִ� �ð��� ������ ����

    public Transform cameraTransform;
    public List<Weapon> weapons; // ���� ���⸦ ������ �ֱ� ���� List ����
    int currentWeaponIndex; // ���� ������ �ִ� ���� �ε���
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
        // Ŀ�� ����
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ���� ȭ���� ����� ���ϵ���
        Cursor.visible = false; // Ŀ�� �����

        // New Input System �ʱ�ȭ (Project Settings - Input System Package)
        InputActionAsset inputActions = GetComponent<PlayerInput>().actions;
        moveAction = inputActions.FindAction("Move"); // Move�� �ش��ϴ� �Է°� ��������
        lookAction = inputActions.FindAction("Look"); // Look�� �ش��ϴ� �Է°� ��������
        fireAction = inputActions.FindAction("Fire");
        reloadAction = inputActions.FindAction("Reload");
        horizontalAngle = transform.localEulerAngles.y; // �ٶ󺸴� ���� ��������

        // ���� �ʱ�ȭ
        verticalAngle = 0;
        verticalSpeed = 0;
        groundTimer = 0;
        isGrounded = true;
        
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        /// Move
        // �÷��̾� �̵� ����
        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        // Vector2�� x,y���� ������� y���� �������̹Ƿ� y���� z�� ��ȯ�ؼ� ���
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y); 

        // �̵� ���Ͱ� 1���� ũ�� 1�� ���� (�밢�� �̵��� ��Ʈ2��ŭ �̵��ǹǷ�)
        if (move.magnitude > 1) move.Normalize();

        move = move * walkingSpeed * Time.deltaTime;
        move = transform.TransformDirection(move); // ���� gameObject�� �������� ���͸� ������
        characterController.Move(move); // Character Controller���� �̵� ��� ����

        /// Look
        // ���콺 �¿�ȸ��
        Vector2 look = lookAction.ReadValue<Vector2>(); // look �׼� ���͸� �����´�
        float turnPlayer = look.x * mouseSens; // �¿� ȸ������ ���콺 ���� ����
        horizontalAngle += turnPlayer; // ���� ������ ȸ������ ����
        if (horizontalAngle >= 360) horizontalAngle -= 360; // 360�� �̻��϶� 360��ŭ ����
        if (horizontalAngle < 0) horizontalAngle += 360; // 0�� �����϶� 360��ŭ ����
        
        Vector3 currentAngle = transform.localEulerAngles; // ��ȭ�� ���� ������ ����
        currentAngle.y = horizontalAngle;
        transform.localEulerAngles = currentAngle;

        // ���콺 ����ȸ��
        float turnCam = look.y * mouseSens; // ���� ȸ������ ���콺 ���� ����
        verticalAngle -= turnCam; // ���ΰ��� ī�޶� ȸ������ ����
                                  // ���콺�� ���� �ø��� ������� �����µ� ���� ������ ������ �־�����ؼ�
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f); // 90������ ���̸� �ȵǴ� �ִ밪, �ּҰ� ����
        currentAngle = cameraTransform.localEulerAngles; // ��ȭ�� ���� ������ ī�޶� ��ġ�� ����
        currentAngle.x = verticalAngle; // �þ߸� �ø��� ����
        cameraTransform.localEulerAngles = currentAngle;

        // �߷�
        verticalSpeed -= gravity * Time.deltaTime; // �ӵ� = ���ӵ� * �ð�

        if (verticalSpeed < -terminalSpeed) // ���ϼӵ��� ���ܼӵ� ���밪���� ���� ���� ������
        {
            verticalSpeed = -terminalSpeed; // ���ϼӵ��� ���ܼӵ� ���밪���� ����
        }
        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0); // ���Ϻ��� ����
        verticalMove *= Time.deltaTime; // ���Ϳ� deltaTime ����

        CollisionFlags flag = characterController.Move(verticalMove); // ĳ���Ϳ��� ���� ����

        if ((flag & (CollisionFlags.Below | CollisionFlags.Above)) != 0) // ĳ������ flag�� Below ��Ʈ�� ������ (�������� ���°� �ƴϸ�), Above�� ���������� �Ӹ� �� ������Ʈ�� �ε��������� ó��
        {
            verticalSpeed = 0; // ���ϼӵ��� 0���� �ʱ�ȭ
        }

        // ����
        /// ���ٴڿ� ��� ������ CollisionFlag Below�� True�� ���;� ������
        /// �Ϻ��� �ùķ��̼��� �ƴϱ� ������ ���� ����־ True�� False�� ������ ���´�.
        /// ó����� => '�����ð����� Ȯ���ϰ�' ���߿� ������ ���� ��� ���� ������ ���ش�.
        if (!characterController.isGrounded) // ������ ���
        {
            if (isGrounded) // true�� �����ϰ�, ���ÿ� ���� ���¿� ���ϱ� ����
            {
                groundTimer += Time.deltaTime; // ���� �������� �ð��� ����ġ�� �ְ�
                if (groundTimer > 0.3f) // 0.3�ʸ� �Ѿ��
                {
                    isGrounded = false; // �����̶�� �Ǵ�
                }
            }
        }
        else // ������ ���
        {
            isGrounded = true; // �����̶�� �Ǵ�
            groundTimer = 0; // ����ġ �ʱ�ȭ
        }

        // ��
        if (fireAction.WasPressedThisFrame()) // �̹� �������� Fire�� �ߴ°�?
        {
            weapons[currentWeaponIndex].FireWeapon();
        }
        if (reloadAction.WasPressedThisFrame()) // �̹� �������� Fire�� �ߴ°�?
        {
            weapons[currentWeaponIndex].ReloadWeapon();
        }
    }

    public void OnChangeWeapon()
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false); // ���� ���� ��Ȱ��ȭ

        currentWeaponIndex++; // ������ �ִ� ������ �ε����� ����
        if (currentWeaponIndex > weapons.Count - 1) // �ε����� ������ �ʰ��� ��� ����
        {
            currentWeaponIndex = 0;
        }
        weapons[currentWeaponIndex].gameObject.SetActive(true); // �� ���� Ȱ��ȭ
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
