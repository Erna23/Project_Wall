using UnityEngine;

public class WallJump : MonoBehaviour
{
    // ���� �� ���� ���� ����
    public float stat_JumpHigh;         // �Ϲ� ���� ����
    public float stat_Charge_JumpHigh; // ���� ���� ����
    public float stat_JumpSpeed;        // �¿� �̵� �ӵ�
    public float stat_Charge_StaminaUse; // ���� �� ���¹̳� �Ҹ�
    public bool isCharging = false;          // ���� ���� Ȯ��

    // ���¹̳� ���� ����
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f;  // ü�� �� �ʴ� ȸ���ϴ� ���¹̳�
    public float staminaUseRate = 10f;   // ���� �� �Ҹ�Ǵ� ���¹̳�

    // ���� ���� �� ������Ʈ ����
    public Rigidbody2D rb;
    public GameObject chargeArrow;

    // ȭ��ǥ�� ĳ���Ϳ��� ������ �Ÿ� (1cm = 0.01 ����)
    public Vector2 arrowOffset = new Vector2(0, 0.01f);  // ȭ��ǥ�� ĳ���� �� 1cm ��ġ

    // ������ ��� ���� ����
    private Vector2 jumpDirection;
    private float jumpTime;
    private bool isWallJumping = false;
    private bool isWallOnLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        chargeArrow = GameObject.FindWithTag("ChargeArrow"); // ���� ���� ȭ��ǥ ã��
        chargeArrow.SetActive(false);  // ó���� ��Ȱ��ȭ

        currentStamina = maxStamina;  // ���¹̳� �ʱ�ȭ
    }

    void Update()
    {
        // ���¹̳� ȸ�� �� �Ҹ� ����
        ManageStamina();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = false;
            Jump(true);  // �Ϲ� ����
            Debug.Log("�Ϲ� ����");
        }

        // ���� ���� ó��
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isCharging = true;
            chargeArrow.SetActive(true); // ���� ���� ȭ��ǥ Ȱ��ȭ
            UpdateArrowPosition();  // ȭ��ǥ ��ġ ������Ʈ
            currentStamina -= Time.deltaTime * stat_Charge_StaminaUse; // ���� ���¹̳� �Ҹ�
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isCharging)
        {
            if (currentStamina > 0)
            {
                isCharging = true;      // ���� ����
            }
            else
                isCharging = false;     // ���¹̳� ���� �� �Ϲ� ����
            
            Jump(true);
            Debug.Log(currentStamina > 0 ? "���� ����" : "���¹̳� �������� �Ϲ� ����");
            chargeArrow.SetActive(false); // ���� ���� ���� �� ȭ��ǥ ��Ȱ��ȭ 
            isCharging = false;
        }

        // ������ � ó��
        if (isWallJumping)
        {
            jumpTime += Time.deltaTime;     // �̵��ӵ� ��ȭ �� �� ���� �����ϸ� ��!!
            Vector2 displacement = jumpDirection * jumpTime + 0.5f * Physics2D.gravity * Mathf.Pow(jumpTime, 2);
            rb.velocity = new Vector2(displacement.x, displacement.y);
        }
    }

    void Jump(bool isCharge)
    {
        // ������ �� �¿� ���� ����
        float direction = isWallOnLeft ? 1f : -1f;  // ���� ���� ������ ����������, ������ ���� ������ �������� ����

        // ���� ���� ����
        float jumpHeight = isCharging ? stat_Charge_JumpHigh : stat_JumpHigh;
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);

        // ���� �ӵ� ��� (�¿� �̵� �ӵ�)
        float horizontalSpeed = stat_JumpSpeed * direction;  // ���⿡ �°� ���� �ӵ� ����

        // ���� ���� ���� (����, ����)
        jumpDirection = new Vector2(horizontalSpeed, jumpVelocity);  // jumpDirection�� ����/���� ���� �ݿ�

        // ���� �ð� �ʱ�ȭ
        jumpTime = 0f;

        isWallJumping = true;
    }

    void ManageStamina()
    {
        // ���� �پ����� ���� �� ���¹̳� ȸ��
        if (isWallJumping)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }
        else
        {
            // ���� �پ��ִ� ���¶�� ���¹̳� �Ҹ�
            currentStamina -= staminaUseRate * Time.deltaTime;
        }

        // ���¹̳��� 0�� �Ǹ� �߶�
        if (currentStamina <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -5f);  // ������ �߶�
        }

        // ���¹̳� �� ���� (0 �̻�, �ִ밪 ����)
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isWallJumping = false; // ���� ����� �� ������ � ����
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isWallOnLeft = collision.transform.position.x < transform.position.x;   // ���� ���ʿ� ������ true
            // ���� �پ����� �� õõ�� �ϰ��ϸ鼭 �¿� �̵� ����
            rb.velocity = new Vector2(rb.velocity.x, -0.5f);
        }
    }

    // ȭ��ǥ ��ġ ������Ʈ: ĳ���� ��ġ���� 1cm ���� ��ġ
    void UpdateArrowPosition()
    {
        if (chargeArrow != null)
        {
            chargeArrow.transform.position = (Vector2)transform.position + arrowOffset;
        }
    }
}