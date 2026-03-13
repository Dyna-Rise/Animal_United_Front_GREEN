using UnityEngine;

public class EnemyDive : MonoBehaviour
{
    public float speed = 3f;
    public float detectDistance = 5f;

    Transform player;
    Vector2 moveDirection;
    bool changedDirection = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 最初は左に飛ぶ
        moveDirection = Vector2.left;
    }

    void Update()
    {
        // プレイヤーを見つけたら1回だけ方向変更
        if (!changedDirection)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < detectDistance)
            {
                moveDirection = (player.position - transform.position).normalized;
                changedDirection = true;
            }
        }

        // 移動
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}
