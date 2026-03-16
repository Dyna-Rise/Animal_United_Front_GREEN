using UnityEngine;

public class BossSlashController : MonoBehaviour
{
    private const float defaultAttackInterval = 1.0f;
    private const int defaultAttackPower = 1;

    public GameObject slashPrefub;
    public GameObject gate;


    public void Slash(int attackPower = defaultAttackPower, float attackinterval = defaultAttackInterval)
    {
        Debug.Log("Slash");

        // Slash のプレハブからオブジェクトを生成。位置は攻撃起点の gate にする。
        // 位置はBOSSに併せたいので、gate を親に設定する。
        GameObject slash = Instantiate(
            slashPrefub,
            gate.transform.position,
            Quaternion.identity,
            gate.transform
        );
        slash.GetComponent<BossSlashManager>().SetInitialize(GetComponent<BossController>());
        slash.SetActive(true);

    }
}
