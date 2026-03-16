using UnityEngine;

public class BossShotManager : MonoBehaviour
{
    public float eraseTime = 10.0f;
    public GameObject hitEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, eraseTime);
    }

    // Update is called once per frame
    void Update()
    {

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
