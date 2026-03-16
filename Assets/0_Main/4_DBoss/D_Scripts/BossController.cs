using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class BossController : MonoBehaviour
{
    // 指定するときに値を固定するために設定
    enum directionName
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public BossSlashController slashController;
    public BossShotController shotController;

    private int hp;
    public int HP
    {
        get { return hp; }
    }


    public GameObject player;
    public GameObject body;

    public GameObject slashPrefub;
    public GameObject shotPrefub;
    public Transform gate;

    // BOSSが引っ込むＹ座標
    public float topLineY = 8.0f;

    public int defaultHP = 20;
    public int defaultAttackPower = 1;
    public float attackRange = 2.5f;

    public float damegeInterval = 3.0f;


    public int laneNum = 7;

    private bool onMove = false;
    private bool onDisplay = false;
    private bool onAttack = false;
    private bool onDamage = false;

    private Vector3 bossSize;   // 位置調整のために使用するので、BOSSのサイズを取得しておく

    private float laneWidth;
    private float radexTime = 0.1f; // 移動は0.1秒ごとに処理する
    private float onDamageTime = 0;


    // この下は仮で作成、後で消す
    float cntTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = defaultHP;
        bossSize = body.GetComponent<Renderer>().bounds.size;

        
        float stageWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).y + Camera.main.ViewportToWorldPoint(new Vector3(-1, 0, 0)).y;
        laneWidth = stageWidth / (laneNum + 2); // レーン数でそのまま除算すると端になってしまうため、２を加算してから除算計算する
    }

    // Update is called once per frame
    void Update()
    {
        // ダメージ中か判定して処理する
        if (onDamageTime >= 0)
        {
            onDamageTime -= Time.deltaTime;
            if (onDamageTime < 0)
            {
                onDamageTime = 0;
                onDamage = false;
            }
        }


        cntTime += Time.deltaTime;

        if (cntTime > 5 && !(onMove))
        {
            cntTime = 0;
            if (onDisplay)
            {
                if (!(onAttack))
                {
                    if (Random.Range(0, 100) % 2 == 0)
                    {
                        onMove = true;
                        Debug.Log("raige");
                        StartCoroutine(MoveRaise(3));
                    }
                    else
                    {
                        onAttack = true;
                        Debug.Log("attack");
                        Attack();
                    }
                }
            }
            else
            {
                onMove = true;
                Debug.Log("drop");
                transform.position = new Vector3(
                                            (Random.Range(0, laneNum) * laneWidth) + laneWidth,
                                            topLineY + bossSize.y,
                                            transform.position.z);
                Vector3 endPos = new Vector3(
                    transform.position.x,
                    3,
                    transform.position.z);
                StartCoroutine(MoveDrop(transform.position, endPos, 3));
            }
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            if(!(onDamage))
            {
                onDamage = true;

                hp -= 1;

                onDamageTime = damegeInterval;
            }
        }
    }


    void ActionRoutine()
    {

    }

    private void Attack()
    {
        if (Vector2.Distance(transform.position,player.transform.position) >= attackRange)
        {
            shotController.ShotTrident(player.transform);
        }
        else
        {
            slashController.Slash(player.transform);
        }
    }

    // 攻撃が終了したときに、向こうからフラグをfalseにする用のメソッド
    public void EndAttack()
    {
        onAttack = false;
    }

    // 引数のオブジェクトと取得したい方向から、画面橋の座標を計算するメソッド
    private Vector3 GetEndPos(Vector3 vec, directionName direction)
    {
        // 引数のオブジェクトの座標を、ViewPort座標へ変換する
        Vector3 viewPos = Camera.main.WorldToViewportPoint(vec);

        // 引数の方向から、オブジェクトの上下左右の端のViewPort座標を設定する
        switch (direction)
        {
            case directionName.Top:
                viewPos = new Vector3(
                    viewPos.x,
                    1,
                    viewPos.z);
                break;

            case directionName.Bottom:
                viewPos = new Vector3(
                    viewPos.x,
                    -1,
                    viewPos.z);
                break;

            case directionName.Left:
                viewPos = new Vector3(
                    -1,
                    viewPos.x,
                    viewPos.z);
                break;

            case directionName.Right:
                viewPos = new Vector3(
                    1,
                    viewPos.x,
                    viewPos.z);
                break;
        }

        // 計算したViewPort座標をWorld座標へ変換する
        Vector3 directionPos = Camera.main.ViewportToWorldPoint(viewPos);

        return directionPos;
    }

    IEnumerator MoveRaise(float completeTime)
    {
        Debug.Log("MoveRaise");

        onMove = true;

        Vector3 endPos = new Vector3(
            transform.position.x,
            topLineY + (bossSize.y / 2),
            transform.position.z);

        for (float i = 0; i <= 1; i += radexTime)
        {
            transform.position = Vector3.Lerp(transform.position, endPos, i);
            yield return new WaitForSeconds(completeTime * radexTime);
        }

        onMove = false;
        onDisplay = false;
    }


    // 下に出てくる動き
    IEnumerator MoveDrop(Vector3 startPos, Vector3 endPos, float completeTime)
    {
        Debug.Log("move arrival");
        onMove = true;

        yield return StartCoroutine(FirstDrop(startPos));

        // 移動は0.1秒ごとに実施する
        float waitTime = completeTime * radexTime;

        for (float i = 0; i <= 1; i += radexTime)
        {
            yield return new WaitForSeconds(waitTime);
            transform.position = Vector3.Lerp(startPos, endPos, i);
        }
        
        onMove = false;
        onDisplay = true;
    }

    IEnumerator FirstDrop(Vector3 pos)
    {
        Debug.Log("Firstdrop");
        Vector3 endPos = GetEndPos(pos, directionName.Top);

        for (float i = 0; i <= 1; i += radexTime)
        {
            yield return new WaitForSeconds(0.1f);
            transform.position = Vector3.Lerp(pos, endPos, i);
        }

        yield return new WaitForSeconds(0.5f);

        for (float i = 1; i >= 0; i -= radexTime)
        {
            yield return new WaitForSeconds(0.1f);
            transform.position = Vector3.Lerp(pos, endPos, i);
        }
    }

    IEnumerator WaitTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(new Vector3(0, topLineY, 0), new Vector3(30, 0.1f, 3));

        //Gizmos.color = new Color(1f, 0, 1f, 0.2f);
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)), new Vector3(30, 0.1f, 3));
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0, -1, 0)), new Vector3(30, 0.1f, 3));
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)), new Vector3(0.1f,30, 3));
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(-1, 0, 0)), new Vector3(0.1f, 30f, 3));

    }




}
