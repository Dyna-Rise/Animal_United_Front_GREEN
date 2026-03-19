using Unity.Mathematics;
using UnityEngine;

public class BossSlashManager : MonoBehaviour
{
    private const int defaultAttackPower = 1;
    private const float defaultAttackInterval = 1.0f;
    private GameObject hitEffect;

    [SerializeField]
    private int attackPower = 1;
    public int AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }


    private BossController boss;
    private float attackInterval;
    private bool isInitialize = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 生成されたタイミングで、攻撃終了のメソッドを呼び出す
        Invoke("EndSlash", attackInterval);
    }

    private void EndSlash()
    {
        Debug.Log("EndSlash");
        // 自身を破棄する前に、Bossに終了することを通知する
        boss.EndAttack();
        Destroy(gameObject);    // 自身を破棄する
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && hitEffect != null)
        {
            GameObject effect = Instantiate(
                hitEffect,
                transform.position,
                quaternion.identity
            );

            Destroy(effect, 1.0f);
        }
    }

    public void SetInitialize(BossController controller, int attackPower = defaultAttackPower, float attackInterval = defaultAttackInterval)
    {
        isInitialize = true;
        SetBoss(controller);
        SetAttack(attackPower, attackInterval);
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

    public void SetAttack(int ackPow, float ackInt)
    {
        attackPower = ackPow;
        attackInterval = ackInt;
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         Instantiate(hitEffect, transform.position, Quaternion.identity);
    //         Destroy(gameObject);
    //     }
    // }

}
