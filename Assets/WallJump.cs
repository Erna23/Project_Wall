using UnityEngine;

public class WallJump : MonoBehaviour
{
    // 점프 및 차지 관련 변수
    public float stat_JumpHigh;         // 일반 점프 높이
    public float stat_Charge_JumpHigh; // 차지 점프 높이
    public float stat_JumpSpeed;        // 좌우 이동 속도
    public float stat_Charge_StaminaUse; // 차지 중 스태미나 소모
    public bool isCharging = false;          // 차지 상태 확인

    // 스태미나 관련 변수
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f;  // 체공 중 초당 회복하는 스태미나
    public float staminaUseRate = 10f;   // 행잉 중 소모되는 스태미나

    // 내부 상태 및 컴포넌트 참조
    public Rigidbody2D rb;
    public GameObject chargeArrow;

    // 화살표가 캐릭터에서 떨어질 거리 (1cm = 0.01 단위)
    public Vector2 arrowOffset = new Vector2(0, 0.01f);  // 화살표가 캐릭터 위 1cm 위치

    // 포물선 운동을 위한 변수
    private Vector2 jumpDirection;
    private float jumpTime;
    private bool isWallJumping = false;
    private bool isWallOnLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        chargeArrow = GameObject.FindWithTag("ChargeArrow"); // 차지 상태 화살표 찾기
        chargeArrow.SetActive(false);  // 처음엔 비활성화

        currentStamina = maxStamina;  // 스태미나 초기화
    }

    void Update()
    {
        // 스태미나 회복 및 소모 관리
        ManageStamina();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = false;
            Jump(true);  // 일반 점프
            Debug.Log("일반 점프");
        }

        // 차지 점프 처리
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isCharging = true;
            chargeArrow.SetActive(true); // 차지 상태 화살표 활성화
            UpdateArrowPosition();  // 화살표 위치 업데이트
            currentStamina -= Time.deltaTime * stat_Charge_StaminaUse; // 차지 스태미나 소모
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isCharging)
        {
            if (currentStamina > 0)
            {
                isCharging = true;      // 차지 점프
            }
            else
                isCharging = false;     // 스태미나 부족 시 일반 점프
            
            Jump(true);
            Debug.Log(currentStamina > 0 ? "차지 점프" : "스태미나 부족으로 일반 점프");
            chargeArrow.SetActive(false); // 차지 상태 종료 후 화살표 비활성화 
            isCharging = false;
        }

        // 포물선 운동 처리
        if (isWallJumping)
        {
            jumpTime += Time.deltaTime;     // 이동속도 변화 시 이 숫자 변경하면 됨!!
            Vector2 displacement = jumpDirection * jumpTime + 0.5f * Physics2D.gravity * Mathf.Pow(jumpTime, 2);
            rb.velocity = new Vector2(displacement.x, displacement.y);
        }
    }

    void Jump(bool isCharge)
    {
        // 점프할 때 좌우 방향 설정
        float direction = isWallOnLeft ? 1f : -1f;  // 왼쪽 벽에 있으면 오른쪽으로, 오른쪽 벽에 있으면 왼쪽으로 점프

        // 점프 높이 결정
        float jumpHeight = isCharging ? stat_Charge_JumpHigh : stat_JumpHigh;
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);

        // 수평 속도 계산 (좌우 이동 속도)
        float horizontalSpeed = stat_JumpSpeed * direction;  // 방향에 맞게 수평 속도 설정

        // 점프 방향 설정 (수평, 수직)
        jumpDirection = new Vector2(horizontalSpeed, jumpVelocity);  // jumpDirection에 수평/수직 방향 반영

        // 점프 시간 초기화
        jumpTime = 0f;

        isWallJumping = true;
    }

    void ManageStamina()
    {
        // 벽에 붙어있지 않을 때 스태미나 회복
        if (isWallJumping)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }
        else
        {
            // 벽에 붙어있는 상태라면 스태미나 소모
            currentStamina -= staminaUseRate * Time.deltaTime;
        }

        // 스태미나가 0이 되면 추락
        if (currentStamina <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -5f);  // 빠르게 추락
        }

        // 스태미나 값 제한 (0 이상, 최대값 이하)
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isWallJumping = false; // 벽에 닿았을 때 포물선 운동 종료
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isWallOnLeft = collision.transform.position.x < transform.position.x;   // 벽이 왼쪽에 있으면 true
            // 벽에 붙어있을 때 천천히 하강하면서 좌우 이동 가능
            rb.velocity = new Vector2(rb.velocity.x, -0.5f);
        }
    }

    // 화살표 위치 업데이트: 캐릭터 위치에서 1cm 위에 위치
    void UpdateArrowPosition()
    {
        if (chargeArrow != null)
        {
            chargeArrow.transform.position = (Vector2)transform.position + arrowOffset;
        }
    }
}