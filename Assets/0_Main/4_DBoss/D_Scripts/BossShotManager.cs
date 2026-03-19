using System.Collections;
using UnityEngine;

public class BossShotManager : MonoBehaviour
{
    public float eraseTime = 10.0f; // 弾が消えるまでの時間を設定
    // public float delayTime = 3.0f;  // 弾が消える時間とは別に、攻撃の間隔を設定

    [SerializeField]
    private int attackPower = 1;
    private int AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }

    public GameObject hitEffect = null;

    private BossController boss;
    private bool isInitialize = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EndShot(eraseTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && hitEffect != null)
        {
            GameObject effect = Instantiate(
                hitEffect,
                 transform.position,
                 Quaternion.identity
                );

            Destroy(effect, 1.0f);
        }
    }


    public void SetInitialize(BossController controller)
    {
        isInitialize = true;
        SetBoss(controller);
    }

    // Bossの行動でSlashを生成したとき、ボスの情報を設定する用のメソッド
    public void SetBoss(BossController controller)
    {
        boss = controller;
    }


    public void SetHit(GameObject hit)
    {
        hitEffect = hit;
    }

    IEnumerator EndShot(float erase)
    {
        Debug.Log("EndShot");

        // float diffErase = erase - delay;

        // 自身を破棄する前に、Bossに終了することを通知する
        // yield return new WaitForSeconds(delay);
        // boss.EndAttack();

        yield return new WaitForSeconds(erase);
        // 自身を破棄する
        //Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
