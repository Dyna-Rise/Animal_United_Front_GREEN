using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    PlayerMove playerMove;

    [Header("生成プレハブ・位置")]
    public GameObject shotPrefabs;
    public GameObject gate;

    [Header("生成間隔")]
    public float interval = 1.0f;

    [Header("スピード")]
    public float shotSpeed = 4.0f;

    Coroutine shootCoroutine;


    void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //シュート発動中でなければ
        if (shootCoroutine == null)
            shootCoroutine = StartCoroutine(ShootCol());
    }

    IEnumerator ShootCol()
    {
        //弾を生成
        GameObject obj = Instantiate(
            shotPrefabs,
            gate.transform.position,
            Quaternion.identity
        );

        //Playerの最終的な向いている方向に飛ばす
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(playerMove.LastInputDirection, 0, 0) * shotSpeed, ForceMode.Impulse);
        //インターバル
        yield return new WaitForSeconds(interval);
        shootCoroutine = null;
    }

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }
}
