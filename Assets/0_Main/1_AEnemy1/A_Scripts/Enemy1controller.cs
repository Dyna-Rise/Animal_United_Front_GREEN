using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;          // 移動速度
    public bool isToRight = false;      // true=右向き　false=左向き
    public float revTime = 0;           // 反転時間
    public LayerMask groundLayer;       // 地面レイヤー
    bool onGround = false;              // 地面フラグ
    float time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isToRight)
        {
            transform.localScale = new Vector3(-1, 1, 0);// 向きの変更
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 地上判定
        onGround = Physics.improvedPatchFriction && Physics.CheckSphere(transform.position, 0.1f, groundLayer);
        if (revTime > 0)
        {
            time += Time.deltaTime;
            if (time >= revTime)
            {
                isToRight = !isToRight;     //フラグを反転させる
                time = 0;                   //タイマーを初期化
                if (isToRight)
                {
                    transform.localScale = new Vector3(-1, 1, 0);  // 向きの変更
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 0);   // 向きの変更
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (onGround)
        {
            Rigidbody rbody = GetComponent<Rigidbody>();
            if (isToRight)
            {
                rbody.linearVelocity = new Vector3(speed, rbody.linearVelocity.y);
            }
            else
            {
                rbody.linearVelocity = new Vector3(-speed, rbody.linearVelocity.y);
            }
        }
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        isToRight = !isToRight;     
        time = 0;                   
        if (isToRight)
        {
            transform.localScale = new Vector3(-1, 1, 0); 
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 0); 
        }
    }
}
