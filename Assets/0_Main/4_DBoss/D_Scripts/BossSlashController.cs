using UnityEngine;

public class BossSlashController : MonoBehaviour
{

    public GameObject slashPrefub;
    public GameObject gate;
    public float AttackTime = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Slash(Transform target)
    {

        Debug.Log("slash");

        // Slash のプレハブからオブジェクトを生成。位置は攻撃起点の gate にする。
        // 位置はBOSSに併せたいので、gate を親に設定する。
        GameObject slash = Instantiate(
            slashPrefub,
            gate.transform.position,
            Quaternion.identity,
            gate.transform
        );
        slash.GetComponent<BossSlashManager>().SetBoss(gameObject.GetComponent<BossController>());
        slash.SetActive(true);

    }
}
