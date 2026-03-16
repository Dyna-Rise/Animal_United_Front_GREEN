using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossController : MonoBehaviour
{

    // BOSSが引っ込むＹ座標
    public float topLineY = 8.0f;

    public int defaultHP = 20;
    public int defaultAttackPower = 1;

    private int hp;
    public int HP
    {
        get { return hp; }
    }


    public GameObject body;

    public GameObject slashPrefub;
    public GameObject shotPrefub;
    public Transform gate;

    private bool onAttack = false;
    private bool onDamage = false;

    // この下は仮で作成、後で消す
    float cntTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = defaultHP;
        StartCoroutine(MoveArrival(3));
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        cntTime += Time.deltaTime; 

    }

    private void AttackSlash()
    {
        if (!(onAttack))
        {
            // Slash のプレハブからオブジェクトを生成。位置は攻撃起点の gate にする。
            // 位置はBOSSに併せたいので、gate を親に設定する。
            GameObject slash = Instantiate(
                                    slashPrefub,
                                    gate.position,
                                    Quaternion.identity,
                                    gate.transform);
            // 生成したshotオブジェクトにBOSS情報を設定する
            slash.GetComponent<BossSlashManager>().SetBoss(gameObject.GetComponent<BossController>());
            slash.SetActive(true);
            onAttack = true;
        }
    }

    private void AttackShotTrident()
    {
        if (!(onAttack))
        {
            // Shot のプレハブからオブジェクトを生成。位置は攻撃起点の gate にする。
            // 位置は独立させたいので、親は設定しない。
            GameObject shot = Instantiate(
                                    shotPrefub,
                                    gate.position,
                                    Quaternion.identity);
            // 生成したshotオブジェクトにBOSS情報を設定する
            shot.GetComponent<BossSlashManager>().SetBoss(gameObject.GetComponent<BossController>());
            shot.SetActive(true);
            onAttack = true;
        }
    }

    IEnumerator MoveArrival(float bottom)
    {
        float compTime = 0.1f;
        Debug.Log("move arrival");

        Vector3 topend = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 objectTop = new Vector3(    topend.y, 
                                            1,
                                            topend.z);



        Vector3 startPos = new Vector3( transform.position.x,
                                        topLineY,
                                        transform.position.z);

        Vector3 endPos = new Vector3(transform.position.x,
                                        bottom,
                                        transform.position.z);

        for (float i = 0; i <= 1; i += compTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForSeconds(i);
            //Debug.Log(i);
        }
    }


    // 攻撃が終了したときに、向こうからフラグをfalseにする用のメソッド
    public void EndAttack()
    {
        onAttack = false;
    }


}
