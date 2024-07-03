using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBullet : MonoBehaviour
{
    private IObjectPool<EnemyBullet> managedPool;

    public void SetManagedPool(IObjectPool<EnemyBullet> pool)
    {
        managedPool = pool;
    }

    public void ReleaseToPool()
    {
        GoliatPool.ReturnObject(this);
    }

    void OnEnable()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReleaseToPool();
    }
}
