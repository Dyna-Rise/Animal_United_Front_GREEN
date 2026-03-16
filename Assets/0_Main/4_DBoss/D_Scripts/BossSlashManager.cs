using UnityEngine;

public class BossSlashManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("setFalse", 5.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setFalse()
    {
        Debug.Log("setFalse");
        gameObject.SetActive(false);
    }
}
