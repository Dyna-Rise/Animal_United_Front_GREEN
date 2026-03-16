using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
    public float attackRange = 3.0f;

    public float damegeInterval = 3.0f;

    public int laneNum = 7;

    private BossSlashController slashController;
    private BossShotController shotController;

    private Vector3 bossSize;   // 位置調整のために使用するので、BOSSのサイズを取得する

    private bool onMove = false;
    private bool onDisplay = false;
    private bool onAttack = false;
    private bool onDamage = false;

    private float laneWidth;
    private float radexTime = 0.1f; // 移動は0.1秒ごとに処理する
    private float damageTime = 0;

    private float cntTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = defaultHP;
        bossSize = body.GetComponent<Renderer>().bounds.size;

        slashController = GetComponent<BossSlashController>();
        shotController = GetComponent<BossShotController>();

        Vector3 localViewPos = Camera.main.WorldToViewportPoint(transform.position);
        float stageWidth = Mathf.Abs(Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, localViewPos.z)).x) + Mathf.Abs(Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, localViewPos.z)).x);
        laneWidth = stageWidth / (laneNum + 2); // レーン数でそのまま除算すると端になってしまうため、２を加算してから除算計算する

        MoveRaise(3);
    }

    // Update is called once per frame
    void Update()
    {
        // ダメージ中か判定して処理する
        if (damageTime >= 0)
        {
            damageTime -= Time.deltaTime;
            if (damageTime < 0)
            {
                damageTime = 0;
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
                    if (Random.Range(0, 100) >= 30)
                    {
                        onMove = true;
                        StartCoroutine(MoveRaise(6));
                    }
                    else
                    {
                        onAttack = true;

                        Attack();
                    }
                }
            }
            else
            {
                onMove = true;


                transform.rotation = ChangeDirection(player.transform);

                transform.position = new Vector3(
                                            (Random.Range(1, laneNum) * laneWidth),
                                            topLineY + bossSize.y,
                                            transform.position.z);

                Vector3 endPos = new Vector3(
                    transform.position.x,
                    Random.Range(1, 6),
                    transform.position.z);

                StartCoroutine(MoveDrop(transform.position, endPos, 3));
            }
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            if (!(onDamage))
            {
                onDamage = true;

                hp -= 1;

                damageTime = damegeInterval;
            }
        }
    }


    void ActionRoutine()
    {

    }

    private void Attack()
    {
        transform.rotation = ChangeDirection(player.transform);
        if (Vector2.Distance(transform.position, player.transform.position) >= attackRange)
        {
            GetComponent<BossShotController>().ShotTrident(player.transform);
        }
        else
        {
            GetComponent<BossSlashController>().Slash(player.transform);
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

    private Quaternion ChangeDirection(Transform target)
    {
        Quaternion directRotation = Quaternion.identity;
        if (transform.position.x > player.transform.position.x)
        {
            // playerが右側にいるので右向き
            return Quaternion.Euler(transform.rotation.x, -90, transform.rotation.z);
        }
        else if (transform.position.x < player.transform.position.x)
        {
            // playerが左側にいるので左向き
            return Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
        }
        else
        {
            return transform.rotation;
        }
    }

    IEnumerator MoveRaise(float completeTime)
    {
        // Debug.Log("MoveRaise");

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
        // Debug.Log("move arrival");
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
        // Debug.Log("Firstdrop");
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

    // デバッグ用
    void OnGUI()
    {
        Vector3 localViewPos = Camera.main.WorldToViewportPoint(transform.position);
        float widthRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, localViewPos.z)).x;
        float widhtLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, localViewPos.z)).x;
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        GUI.Label(new Rect(50, 50, 200, 200),
                    "プレイヤーとの距離 " + Vector2.Distance(transform.position, player.transform.position).ToString(),
                    style);
        GUI.Label(new Rect(50, 150, 200, 200),
         "画面右端 " + widthRight.ToString() + "; 画面左端 " + widhtLeft.ToString(),
          style);
        GUI.Label(new Rect(50, 250, 200, 200), "BOSSの横位置 " + transform.position.x.ToString(), style);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(new Vector3(0, topLineY, 0), new Vector3(30, 0.1f, 3));

        //Gizmos.color = new Color(1f, 0, 1f, 0.2f);
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)), new Vector3(30, 0.1f, 3));
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0, -1, 0)), new Vector3(30, 0.1f, 3));
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)), new Vector3(0.1f,30, 3));
        //Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(-1, 0, 0)), new Vector3(0.1f, 30f, 3));

    }




}
