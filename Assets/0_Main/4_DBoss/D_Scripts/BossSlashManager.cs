using UnityEngine;

public class BossSlashManager : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 1;
    public int AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }
    
    private BossController boss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("instantate Slash");

        // 生成されたタイミングで、攻撃終了のメソッドを呼び出す
        Invoke("EndSlash", 1.0f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void EndSlash()
    {
        Debug.Log("EndSlash");

        // 自身を破棄する前に、Bossに終了することを通知する
        boss.EndAttack();
        Destroy(gameObject);    // 自身を破棄する
    }

    // Bossの行動でSlashを生成したとき、ボスの情報を設定する用のメソッド
    public void SetBoss(BossController controller)
    {
        boss = controller;
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
