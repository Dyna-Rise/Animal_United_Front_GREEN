using System.Collections;
using UnityEngine;

public class Enemy4Controller : MonoBehaviour
{
    GameObject player;

    [Header("体力")]
    public int life = 5;

    [Header("シュートスピード・インターバル")]
    public int shootSpeed = 5;
    public float interval = 3;

    [Header("生成ショット・生成ガード")]
    public GameObject shootPrefab;
    public GameObject gate;
    public GameObject guardPrefab;
    public GameObject guardGate;

    //それぞれの切り替えスイッチ
    bool toGuard, toShoot;

    Coroutine shootCoroutine;
    Coroutine guardCoroutine;

    [Header("ダメージ時間・ダメージ移動量")]
    public float stunTime = 0.2f;
    public float damageSpeed = 0.5f;

    float damageTimer; //ダメージ時間を測るタイマー
    bool isDamage; //ダメージフラグ

    [Header("点滅対象")]
    public GameObject enemyBody;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerFollower");
        toGuard = true; //最初はガードから
    }

    // Update is called once per frame
    void Update()
    {
        //ダメージ中なら減らす
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;

            float val = Mathf.Sin(Time.time * 50);
            if (val > 0) enemyBody.SetActive(true);
            else enemyBody.SetActive(false);

        }
        else if (isDamage)
        {
            enemyBody.SetActive(true);
            isDamage = false;
        }

        if (shootCoroutine == null && toShoot)
        {
            shootCoroutine = StartCoroutine(ShootCol());
        }
        else if (guardCoroutine == null && toGuard)
        {
            guardCoroutine = StartCoroutine(GuardCol());
        }

    }

    IEnumerator ShootCol()
    {
        toShoot = false;
        //弾の生成
        GameObject obj = Instantiate(
            shootPrefab,
            gate.transform.position,
            Quaternion.identity
            );

        obj.GetComponent<Rigidbody>().AddForce(new Vector3(-shootSpeed, 0, 0), ForceMode.Impulse);
        yield return new WaitForSeconds(interval);
        shootCoroutine = null;
        toGuard = true;
    }

    IEnumerator GuardCol()
    {
        toGuard = false;

        //盾の生成
        GameObject obj = Instantiate(
            guardPrefab,
            guardGate.transform.position,
           Quaternion.identity
        );


        yield return new WaitForSeconds(interval);

        Destroy(obj.gameObject);
        guardCoroutine = null;
        toShoot = true;
    }

    //ダメージ
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAttack")
        {
            if (damageTimer <= 0 && !isDamage)
            {
                if((guardCoroutine == null) || (guardCoroutine != null && player.transform.position.x > transform.position.x))
                {
                    life--;
                    if (life <= 0)
                    {
                        Destroy(gameObject);
                    }
                    damageTimer = stunTime;
                    isDamage = true;
                }
            }
        }
    }

}
