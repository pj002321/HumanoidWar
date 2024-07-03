using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEvent : MonoBehaviour
{
    private Rigidbody rb;
    public float moveSpeed = 1.0f; // 이동 속도 조절 변수

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false; // 중력을 사용하지 않도록 설정
    }

    private void Update()
    {
        Vector3 movement = new Vector3(0, -moveSpeed * Time.deltaTime, 0);
        rb.MovePosition(transform.position + movement);
    }
}
