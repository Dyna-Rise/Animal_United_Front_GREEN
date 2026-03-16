using System.Collections;
using UnityEngine;

public class BossShotController : MonoBehaviour
{

    public GameObject defaultShotPrefub;   // 生成するショットの元データ
    public GameObject gate;
    public readonly int defaultShotNum = 3;   // 弾数指定がないときに放つ弾の数
    public readonly float defaultShotSpeed = 1.0f;  // 生成したショットの弾速
    public readonly float defaultDelayTime = 0; // 弾ごとの生成ディレイ時間（デフォルト０、値が大きくなると１発ごとにディレイが発生する）

    public int maxAngle = 150;      // 発射する際の最大角度

    void Start()
    {
        // テスト用
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //ShotTrident(player.transform);
    }

    // 相手だけ指定してデフォルトでショットを実行
    public void ShotTrident(Transform target)
    {
        // オーバーロードを利用して下のメソッドを実行する
        ShotTrident(
            target,
            defaultShotPrefub,
            defaultShotNum,
            defaultShotSpeed,
            defaultDelayTime
            );
    }

    // 弾数指定がないときは、３方向にショットを放つ、１発ごとのディレイを設定できる（デフォルトは０）
    public void ShotTrident(Transform target, GameObject shotPrefub, int shotNum, float shotSpeed, float delayTime)
    {
        // target へのベクトルを計算して正規化する
        float dx = target.position.x - gameObject.transform.position.x;
        float dy = target.position.y - gameObject.transform.position.y;
        //float dz = target.position.z - gameObject.transform.position.z;　// 何かあったときに使うかもなので残しておく。

        // できるだけ発射のタイミングを合わせるため、生成⇒発射で段階ごとに処理を実施する
        GameObject[] shots = new GameObject[shotNum];
        // 弾の生成
        for (int i = 0; i < shotNum; i++)
        {
            shots[i] = Instantiate(
                shotPrefub,
                gate.transform.position,
                Quaternion.identity
                );
            shots[i].GetComponent<BossSlashManager>().SetBoss(gameObject.GetComponent<BossController>());
        }

        // 弾の発射
        StartCoroutine(fireShot(new Vector2(dx, dy), delayTime, shots, shotSpeed));
    }


    // 弾の発射（弾の速度は、正規化されたベクトルをもとにshotSpeedを乗算する）
    IEnumerator fireShot(Vector2 vec, float delay, GameObject[] shots, float shotSpeed)
    {
        float perAngle = 150 / shots.Length;                     // 最大の角度を１５０度として、１弾あたりの角度を計算
        int shotAngle = (int)(shots.Length / 2) * (int)perAngle; // targetを正面として、そこを中心に最初に弾を生成する角度を定義（下から生成する）一番上と下の真ん中の値なので１／２。
                                                                 // 整数値が欲しいので、割った値を int で型変換

        foreach (var shot in shots)
        {
            float rad = Mathf.Atan2(vec.y, vec.x);          // target と発射点（Gate）との角度を計算
            float deg = rad * Mathf.Rad2Deg + shotAngle;    // target との角度に、今回発射する角度を補正
            float rad_x = Mathf.Cos(Mathf.Deg2Rad * deg);   // 補正した角度にcosineでX軸のベクトルを計算
            float rad_y = Mathf.Sin(Mathf.Deg2Rad * deg);   // 補正した角度にsineでY軸のベクトルを計算

            shot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, deg)); // 弾オブジェクトの向きを発射方向に変更
            Vector2 shotDirect = new Vector2(rad_x, rad_y).normalized;          // 発射方向をベクトルに変換して、正規化する

            shot.SetActive(true);   // 初期状態でオブジェクトが無効なので、有効化
            shot.GetComponent<Rigidbody>().AddForce(shotDirect * shotSpeed, ForceMode.Impulse); // 正規化したベクトルに弾速を与えて弾を発射する

            shotAngle -= (int)perAngle; // 次の弾に備えて設定する角度を更新
            yield return new WaitForSeconds(delay);
        }
    }

    // memo
    // 上記はAddForceで直接飛ぶ方向を指定しているが、生成時に角度を指定→Vector3.forwardのように決まった方向に飛ばすのもあり？
    // その場合は弾自体にスクリプトが必要そう。
}
