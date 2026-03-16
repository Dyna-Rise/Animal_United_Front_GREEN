using System.Collections;
using UnityEngine;

public class testscript : MonoBehaviour
{
    Vector3 topendWorld = new Vector3();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 不採用（TOPのラインを決めて移動する）
    void TestCheckTopEnd()
    {
        Vector3 i =  Camera.main.WorldToViewportPoint(transform.position);
        Debug.Log("i " + i);

        Vector3 iv = Camera.main.ViewportToWorldPoint(i);
        Debug.Log("iv "+iv);

        Vector3 topend = new Vector3(
            i.y,
            1,
            i.z
            );

        topendWorld = Camera.main.ViewportToWorldPoint(topend);

        Debug.Log("topendWorld " + topendWorld);
    }

    IEnumerator MoveTopend()
    {
        Debug.Log("call move");
        Debug.Log("topendWorld.y >" + topendWorld.y);
        yield return new WaitForSeconds(2.0f);
        Debug.Log("wait 2s");

        transform.position = new Vector3(
            transform.position.x,
            topendWorld.y + GetComponent<Renderer>().bounds.size.y + 0.1f,
            transform.position.z
            );
    }

}
