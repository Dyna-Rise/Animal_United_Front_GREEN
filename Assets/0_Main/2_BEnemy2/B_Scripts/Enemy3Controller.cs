using UnityEngine;

public class Enemy3Controller : MonoBehaviour
{
    GameObject player;

    [Header("体力・通常スピード・突進スピード")]
    public int life = 2;
    public float speed = 3.0f;
    public float rushSpeed = 5.0f;

    bool isRush; //突進フラグ
    float vx, vy; //突進方向値

    [Header("索敵範囲")]
    public float range = 6.0f;

    [Header("廃棄時間")]
    public float deleteTime = 6.0f;

    [Header("ダメージ時間・ダメージ移動量")]
    public float stunTime = 0.2f;
    public float damageSpeed = 0.5f;

    float damageTimer; //ダメージ時間を測るタイマー
    bool isDamage; //ダメージフラグ

    [Header("点滅対象")]
    public GameObject enemyBody;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerFollower");
        Destroy(gameObject, deleteTime);
    }

    // Update is called once per frame
    void Update()
    {
        //ダメージ中なら減らす
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;

            float val = Mathf.Sin(Time.time * 50);
            if (val > 0) enemyBody.SetActive(true);
            else enemyBody.SetActive(false);

        }
        else if (isDamage)
        {
            enemyBody.SetActive(true);
            isDamage = false;
        }

        //プレイヤーとの直線距離を取得
        float dis = Vector2.Distance(player.transform.position, transform.position);

        //範囲内ならPlayerに突進
        if (dis < range && player != null)
        {
            if (!isRush) //最初一回目だけ目標方向を取得
            {
                //2者間の差をｘ成分（底辺）、ｙ成分（高さ）に分解
                float dx = player.transform.position.x - transform.position.x;
                float dy = player.transform.position.y - transform.position.y;

                //底辺と高さを用いて角度情報を入手（逆タンジェント関数を利用）
                float rad = Mathf.Atan2(dy, dx);

                //角度情報からあらためて、長辺を1とした時のxとyの比率をそれぞれ入手
                vx = Mathf.Cos(rad);
                vy = Mathf.Sin(rad);
                isRush = true;
            }

            //突進する
            if (damageTimer > 0)
                //ダメージ中なら鈍い
                GetComponent<Rigidbody>().linearVelocity = (new Vector3(vx, vy, 0).normalized) * rushSpeed * 0.2f;
            else
                GetComponent<Rigidbody>().linearVelocity = (new Vector3(vx, vy, 0).normalized) * rushSpeed;
        }
        else
        {
            //変数speedの方に動く
            if (damageTimer > 0)
                //ダメージ中なら鈍い
                transform.position += new Vector3(speed * 0.1f, 0, 0) * Time.deltaTime;
            else
                transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;

        }
    }

    //ダメージ
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAttack")
        {
            if (damageTimer <= 0 && !isDamage)
            {
                life--;
                if (life <= 0)
                {
                    Destroy(gameObject);
                }
                damageTimer = stunTime;
                isDamage = true;
            }
        }
    }

    //対象物を選択したときだけGizumoが出る
    void OnDrawGizmosSelected()
    {
        //円（ワイヤー）を表示 (中心から、変数searchRangeの半径で）
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
