using UnityEngine;

public class EnemyDive : MonoBehaviour
{
    public float speed = 3f; //移動スピード
    public float detectDistance = 5f;　//プレイヤーを見つける距離
    public float searchRange;       // 索敵範囲
    public float deleteTime = 20.0f;　//何秒後に自動削除するか

    public int hp = 5;　//HP

    Transform player;　//プレイヤーのTransformを保存
    Vector3 moveDirection;　//進む方向
    bool changedDirection = false;　//方向変更したかどうか（一回だけ変更するため）

    void Start()
    {
        // 一定時間後にこのオブジェクトを削除
        Destroy(gameObject, deleteTime);
        // タグ「Player」のオブジェクトを探してTransformを取得
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 最初は左に飛ぶ
        moveDirection = Vector3.left;
    }

    void Update()
    {
        // プレイヤーを見つけたら1回だけ方向変更
        if (!changedDirection)
        {
            // プレイヤーとの距離を計算
            float distance = Vector2.Distance(transform.position, player.position);
            // プレイヤーが一定距離以内に入ったら
            if (distance < detectDistance)
            {
                // プレイヤーの方向を計算して進行方向にする
                moveDirection = (player.position - transform.position).normalized;
                // 1回だけ方向変更するためフラグをON
                changedDirection = true;
            }
        }

        // 移動
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーの攻撃に当たったら
        if (other.CompareTag("PlayerAttack"))
        {
            hp--;

            Destroy(other.gameObject);

            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
