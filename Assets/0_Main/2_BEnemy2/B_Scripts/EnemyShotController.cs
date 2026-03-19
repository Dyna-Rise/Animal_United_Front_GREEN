using UnityEngine;

public class EnemyShotController : MonoBehaviour
{
    public float deleteTime = 10.0f;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }
}
