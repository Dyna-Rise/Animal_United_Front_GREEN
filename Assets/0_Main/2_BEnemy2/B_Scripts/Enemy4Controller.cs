using UnityEngine;
using System.Collections;

public class Enemy4Controller : MonoBehaviour
{
    public GameObject EnemyGuard;   // 盾プレハブ
    public GameObject EnemyShot;    // 弾プレハブ
    public Transform shotPoint;     // 弾の発射位置

    public float guardTime = 3f;    // 盾を張る時間
    public float shootTime = 3f;    // シュート時間

    public int hp = 5;   // エネミーの体力

    bool guardFlag = true;          // 盾モードのフラグ（最初はON）
    bool shootFlag = false;　　　　 //シュートモードのフラグ　

    Coroutine guardCoroutine = null;　　// 盾コルーチンが動いているか確認
    Coroutine shootCoroutine = null;　　// シュートコルーチンが動いているか確認

    GameObject currentGuard;　// 現在出ている盾を保存

    void Update()
    {
        // 盾モード開始条件
        // 盾フラグON かつ 盾コルーチンが動いていない
        if (guardFlag && guardCoroutine == null)
        {
            guardCoroutine = StartCoroutine(GuardMode());
        }

        // シュートモード開始条件
        // シュートフラグON かつ シュートコルーチンが動いていない
        if (shootFlag && shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(ShootMode());
        }
    }

    IEnumerator GuardMode()
    {
        // 盾モード開始なのでフラグをOFF
        guardFlag = false;

        // 盾プレハブをEnemy4の位置に生成
        currentGuard = Instantiate(EnemyGuard, transform.position, Quaternion.identity);

        // Enemy4の子オブジェクトにする
        // → Enemy4が動いても盾が一緒についてくる
        currentGuard.transform.SetParent(transform);

        // 一定時間待つ
        yield return new WaitForSeconds(guardTime);

        // 盾削除
        if (currentGuard != null)
        {
            Destroy(currentGuard);
        }

        // シュートへ
        shootFlag = true;
        guardCoroutine = null;
    }

    IEnumerator ShootMode()
    {
        // シュートモード開始なのでフラグOFF
        shootFlag = false;

        // 弾を発射位置から生成
        Instantiate(EnemyShot, shotPoint.position, Quaternion.identity);

        // シュートモードの時間待つ
        yield return new WaitForSeconds(shootTime);

        // 次は盾モードへ
        guardFlag = true;
        shootCoroutine = null;　 // コルーチン終了
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーの攻撃に当たったら
        if (other.CompareTag("PlayerAttack"))
        {
            hp--;　//体力減少

            Destroy(other.gameObject);

            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}