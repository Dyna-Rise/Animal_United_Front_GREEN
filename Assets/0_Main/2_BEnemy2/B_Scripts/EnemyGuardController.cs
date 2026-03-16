using UnityEngine;

public class EnemyGuardController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //盾に触れた"PlayerAttack"は全消滅
        if (other.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
        }
    }
}
