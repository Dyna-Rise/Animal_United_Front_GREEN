using UnityEngine;

public class EnemyGauardController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //プレイヤーの攻撃は全て消滅させる
        if (other.gameObject.tag == "PlayerAttack") Destroy(other.gameObject);
    }
}
