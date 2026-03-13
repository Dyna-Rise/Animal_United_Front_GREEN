using UnityEditor;
using UnityEngine;

public class Enemy4Controller : MonoBehaviour

{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}
