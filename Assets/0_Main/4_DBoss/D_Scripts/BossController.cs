using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    public Vector3 defaultPosition = new Vector3(8, 4, 0);
    public int defaultHP = 20;
    public int defaultAttackPower = 1;
    public float attackRange = 3.0f;

    public float damegeInterval = 3.0f; // ダメージ後の無敵時間
    public float actionInterval = 5.0f; // BOSS行動をする間隔

    public float flashingSpeed = 5.0f; // 点滅するスピード
    public int laneNum = 5; // BOSSが移動するレーンの数
    public int maxConsecutive = 5;  // 連続で攻撃をする最大回数

    private BossSlashController slashController;
    private BossShotController shotController;

    private Vector3 bossSize;   // 位置調整のために使用するので、BOSSのサイズを取得する

    private bool onMove = false;
    private bool onDisplay = false;
    private bool onAttack = false;
    private bool onDamage = false;

    private float stageWidth;
    private float laneWidth;
    private float radexTime = 0.1f; // 移動は0.1秒ごとに処理する
    private float damageTime = 0;

    private float cntTime;
    private float cntConsencutive = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Boss Start");
        hp = defaultHP;
        // bossSize = body.GetComponent<Renderer>().bounds.size;

        slashController = GetComponent<BossSlashController>();
        shotController = GetComponent<BossShotController>();

        stageWidth = GetWidthWorldPoint(transform);

        Debug.Log(stageWidth);

        laneWidth = stageWidth / (laneNum + 2); // レーン数でそのまま除算すると端になってしまうため、２を加算してから除算計算する

        cntTime = 0;
        cntConsencutive = 0;

        transform.position = defaultPosition;

        onMove = true;
        StartCoroutine(MoveRaise(3));
    }

    // Update is called once per frame
    void Update()
    {
        // hpが０ならゲームの進行を止める（仮）　ゲームクリアについては差し替えて対応
        if (hp <= 0)
        {
            Destroy(gameObject);
            Time.timeScale = 0;
        }

        // ダメージ中か判定して処理する
        if (damageTime > 0)
        {
            damageTime -= Time.deltaTime;
            if (damageTime <= 0)
            {
                damageTime = 0;
                onDamage = false;
                body.SetActive(true);
            }
            else
            {
                if (Mathf.Sin(Time.time * (flashingSpeed * 2)) > 0)     // flashingSpeedは１秒間で点滅する回数なので、２倍する（点灯消滅で１回ずつ使うため）
                {
                    body.SetActive(false);
                }
                else
                {
                    body.SetActive(true);
                }
            }
        }

        cntTime += Time.deltaTime;

        if (cntTime > actionInterval && !(onMove))
        {
            cntTime = 0;
            if (onDisplay)
            {
                if (!(onAttack))
                {
                    if (UnityEngine.Random.Range(0, 100) >= 30 && cntConsencutive < maxConsecutive)
                    {
                        onAttack = true;
                        cntConsencutive += Attack();
                    }
                    else
                    {
                        onMove = true;
                        cntConsencutive = 0;
                        StartCoroutine(MoveRaise(6));
                    }
                }
            }
            else
            {
                onMove = true;

                transform.position = new Vector3(
                                            (UnityEngine.Random.Range(0, laneNum) * laneWidth) - (laneWidth * (laneNum / 2)),
                                            topLineY,
                                            transform.position.z);

                // transform.position = new Vector3(
                //                             (UnityEngine.Random.Range(1, laneNum) * laneWidth) - (stageWidth / 2) - (bossSize.x / 2),
                //                             topLineY + (bossSize.y / 2),
                //                             transform.position.z);

                Vector3 endPos = new Vector3(
                    transform.position.x,
                    UnityEngine.Random.Range(2.5f, 4.0f),
                    transform.position.z);

                transform.rotation = ChangeDirection(player.transform);
                StartCoroutine(MoveDrop(transform.position, endPos, 3));
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
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

    private int Attack()
    {
        int actionWait = 1;
        transform.rotation = ChangeDirection(player.transform);

        if (Vector2.Distance(transform.position, player.transform.position) >= attackRange)
        {
            GetComponent<BossShotController>().ShotTrident(player.transform);
        }
        else
        {
            GetComponent<BossSlashController>().Slash();
            actionWait = 5;

            WaitTime(1.5f); // 攻撃後硬直
        }
        return actionWait;
    }

    // 攻撃が終了したときに、呼び出したオブジェクトからフラグをfalseにする用のメソッド
    public void EndAttack()
    {
        onAttack = false;
    }

    // ステージの横幅をWorld座標の数値で取得する
    private float GetWidthWorldPoint(Transform target)
    {
        Vector3 right = GetEndPos(new Vector3(1, 0.5f, target.position.z), directionName.Right);
        Vector3 left = GetEndPos(new Vector3(0, 0.5f, target.position.z), directionName.Left);

        float width = Mathf.Abs(right.x) + Mathf.Abs(left.x);
        return width;
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
                    0,
                    viewPos.z);
                break;

            case directionName.Left:
                viewPos = new Vector3(
                    0,
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
            topLineY,
            transform.position.z);

        // Vector3 endPos = new Vector3(
        //     transform.position.x,
        //     topLineY + (bossSize.y / 2),
        //     transform.position.z);

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
        for (float i = 0; i <= 1; i += radexTime)
        {
            yield return new WaitForSeconds(radexTime);
            transform.position = Vector3.Lerp(startPos, endPos, i);
        }

        onMove = false;
        onDisplay = true;
    }

    IEnumerator FirstDrop(Vector3 pos)
    {
        // Debug.Log("Firstdrop");
        Vector3 endPos = GetEndPos(pos, directionName.Top);

        // 移動は0.1秒ごとに実施する
        for (float i = 0; i <= 1; i += radexTime)
        {
            yield return new WaitForSeconds(radexTime);
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
        float widthRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, localViewPos.z)).x;
        float widhtLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, localViewPos.z)).x;
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        GUI.Label(new Rect(50, 50, 200, 200),
                    "プレイヤーとの距離 " + Vector2.Distance(transform.position, player.transform.position).ToString(),
                    style);
        GUI.Label(new Rect(50, 100, 200, 200), "画面幅 " + stageWidth.ToString(), style);
        GUI.Label(new Rect(50, 150, 200, 200),
         "画面右端 " + widthRight.ToString() + "; 画面左端 " + widhtLeft.ToString(),
          style);
        GUI.Label(new Rect(50, 200, 200, 200), "BOSSの横位置 " + transform.position.x.ToString(), style);

        GUI.Label(new Rect(50, 250, 200, 200), "cntTime " + cntTime.ToString() + "; BOSS hp=" + hp + ";", style);

        GUI.Label(new Rect(50, 300, 200, 200), "AttackConsencutive " + cntConsencutive, style);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange); // 近接攻撃の判定距離
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(new Vector3(0, topLineY, 0), new Vector3(30, 0.1f, 3)); // BOSSが上と認識しているライン(ピンク)

        Gizmos.color = new Color(0, 10, 20, 0.2f);  // 映っている範囲のGIZMOSを水色にする
        Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1, 10)), new Vector3(19.5f, 0.1f, 3)); // 映っている範囲（上）
        Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 10)), new Vector3(19.5f, 0.1f, 3)); // 映っている範囲（下）
        Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 10)), new Vector3(0.1f, 11.5f, 3)); // 映っている範囲（右）
        Gizmos.DrawCube(Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 10)), new Vector3(0.1f, 11.5f, 3)); // 映っている範囲（左）

        float sw = GetWidthWorldPoint(transform);
        float lw = sw / (laneNum + 2);
        float def = lw * (int)(laneNum / 2);

        Gizmos.color = new Color(20, 30, 0, 0.2f);
        for (int i = 1; i <= laneNum; i++)
        {
            Gizmos.DrawCube(new Vector3(def, 12, 0), new Vector3(1, 1, 1));
            def -= lw;
        }
        // Laneの表示
    }
}
