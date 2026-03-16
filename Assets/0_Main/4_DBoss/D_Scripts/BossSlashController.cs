using UnityEngine;

public class BossSlashController : MonoBehaviour
{

    public GameObject slashPrefub;
    public float AttackTime = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Slash(player.transform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Slash(Transform target)
    {
        Debug.Log("slash");
        Instantiate(
            slashPrefub,
            transform.position,
            Quaternion.identity
        ).SetActive(true);

    }
}
