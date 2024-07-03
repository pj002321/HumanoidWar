using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyBot : MonoBehaviour
{

    public GameObject BulletObject;
    private float bulletSpeed = 500;
    private float BulletActiveTerm = 0.0f;
    public float radius = 20f;
    private NavMeshAgent agent;
    public LayerMask playerLayer;
    private Vector3 originPosition;
    public int hp;
    private int maxhp = 100;

    private GameObject ExplosObjects;
    public GameObject ExplosionPreb;

    private float ParticleForce = 5;
    private float LifeTime = 5.0f;
    RaycastHit hit;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originPosition = transform.position;
    }

    void Update()
    {
        if (!IsAlive)
        {
            ExplosionAction();
            Destroy(gameObject,0.5f);
        }
        else
        {
            Collider[] playerInRange = Physics.OverlapSphere(transform.position, radius, playerLayer);
            if (playerInRange.Length > 0)
            {
                BulletActiveTerm += Time.deltaTime * 1.0f;

                Vector3 playerPosition = playerInRange[0].transform.position;
                transform.LookAt(playerPosition, Vector3.up);
                if (Physics.Raycast(transform.position, transform.forward, out hit, 15.0f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        agent.speed = 0.0f;
                        BulletAction(transform.position, transform.forward);
                    }
                }
                else
                {
                    agent.speed = 1.5f;
                    agent.SetDestination(playerPosition);

                }

            }
            float newY = Mathf.Sin(Time.time * 1.0f) * 0.5f + originPosition.y;
            Vector3 position = agent.transform.position;
            position.y = newY;
            agent.transform.position = position;
        }
    }
    void ExplosionAction()
    {

        for (int i = 0; i < 40; i++)
        {
            Vector3 StartPos = transform.position;
            ExplosObjects = Instantiate(ExplosionPreb, new Vector3(StartPos.x, StartPos.y, StartPos.z), Quaternion.identity);
            ExplosObjects.transform.Rotate(10, 10, 10);
            Rigidbody Exrigidby = ExplosObjects.GetComponent<Rigidbody>();

            Vector3 RandomDir = Random.insideUnitSphere * ParticleForce;
            Exrigidby.AddForce(RandomDir * 10, ForceMode.Impulse);
            Destroy(ExplosObjects, LifeTime);
        }

    }
    private bool IsAlive => hp > 0;
    void BulletAction(Vector3 spawnPosition, Vector3 direction)
    {

        GameObject bullet = Instantiate(BulletObject, new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z), Quaternion.identity);
        bullet.transform.forward = direction;
        bullet.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(direction * bulletSpeed, ForceMode.Force);
        Destroy(bullet, 2.0f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Foot"))
        {
            
            hp -= 100;
        }
    }
}
