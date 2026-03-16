using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;          // 移動速度
    public bool isToRight = false;      // true=右向き false=左向き
    public LayerMask Ground;       // 地面レイヤー

    public Transform groundCheck;       // 自分の足元
    public Transform cliffCheck;        // 前方の足元
    public float groundCheckRadius = 0.15f;
    public float cliffCheckDistance = 0.5f;

    bool onGround = false;
    bool groundAhead = true;

    Rigidbody rbody;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        UpdateFacing();
    }

    void Update()
    {
        // 足元に地面があるか
        if (groundCheck != null)
        {
            onGround = Physics.CheckSphere(
                groundCheck.position,
                0.3f,
                Ground
            );
        }

        // 前方足元に地面があるか（崖チェック）
        if (cliffCheck != null)
        {
            groundAhead = Physics.Raycast(
                cliffCheck.position,
                Vector3.down,
                cliffCheckDistance,
                Ground
            );
        }

        // 地面の上にいて、前方足元に地面がなければ反転
        if (onGround && !groundAhead)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (!onGround) return;

        float dir = isToRight ? 1f : -1f;

        rbody.linearVelocity = new Vector3(
            dir * speed,
            rbody.linearVelocity.y,
            rbody.linearVelocity.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // 壁用Triggerに触れたときだけ反転
        if (other.CompareTag("Wall"))
        {
            Flip();
        }
    }

    void Flip()
    {
        isToRight = !isToRight;
        UpdateFacing();
    }

    void UpdateFacing()
    {
        if (isToRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (cliffCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                cliffCheck.position,
                cliffCheck.position + Vector3.down * cliffCheckDistance
            );
        }
    }
}