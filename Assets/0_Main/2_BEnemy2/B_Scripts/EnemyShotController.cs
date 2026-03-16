using UnityEngine;

public class EnemyShotController : MonoBehaviour
{
    public float speed = 5f;
    public float deleteTime = 5f; // 何秒後に弾を消すか

    void Update()
    {
        // 一定時間後に弾を自動削除
        Destroy(gameObject, deleteTime);
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    // 何かに触れたとき
    void OnTriggerEnter(Collider other)
    {
        // 触れた相手がPlayerなら
        if (other.CompareTag("Player"))
        {
            // 弾を消す
            Destroy(gameObject);
        }
    }
}
