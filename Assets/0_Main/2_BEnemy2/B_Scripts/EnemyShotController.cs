using UnityEngine;

public class EnemyShotController : MonoBehaviour
{
    public float deleteTime;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }
}
