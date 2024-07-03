using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Goliathstate : MonoBehaviour
{
    public GameObject bulletObject;
    private float bulletSpeed = 1000;
    public Transform lazerPosion;
    public float radius = 10f;
    private GameObject ExplosObjects;
    public GameObject ExplosionPreb;
    public ParticleSystem particlePrefab;
    private float ParticleForce = 5;
    private float LifeTime = 0.5f;
    public Animator anim;
    private NavMeshAgent agent;
    public LayerMask playerLayer;
    private AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip attackClip;
    private Rigidbody rb;
    private Slider slider;
    public int hp;
    private int maxhp = 100;

    private int hashMove = Animator.StringToHash("IsMove");
    private int hashLooking = Animator.StringToHash("IsLooking");
    RaycastHit hit;

    void Start()
    {
        rb=GetComponent<Rigidbody>();
        slider = GetComponentInChildren<Slider>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        audioSource=GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!IsAlive)
        {
            agent.speed = 0f;
            anim.SetBool("IsAlive",false);
            anim.SetBool(hashLooking, false);
            anim.SetBool(hashMove, false);
            OnDyingAnimationEnd();
            slider.value = 0f;
        }
        else
        {
            slider.value = hp * 0.01f;
            Collider[] playerInRange = Physics.OverlapSphere(transform.position, radius, playerLayer);

            if (playerInRange.Length > 0)
            {
                Transform playerTransform = playerInRange[0].transform;
                if (Physics.Raycast(lazerPosion.transform.position, transform.forward, out hit, 5.0f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        agent.speed = 0f;
                        anim.SetBool(hashMove, false);
                        anim.SetBool(hashLooking, true);
                        BulletAction(lazerPosion.transform.position, transform.forward);
                    }
                }
                else
                {
                    agent.speed = 1.5f;
                    agent.SetDestination(playerTransform.transform.position);
                    Vector3 targetDirection = (playerTransform.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);

                    anim.SetBool(hashLooking, false);
                    anim.SetBool(hashMove, true);
                }
            }
        }
    }
    private bool IsAlive => hp > 0;
    void ExplosionAction()
    {

        for (int i = 0; i < 60; i++)
        {
            Vector3 StartPos = transform.position;
            ExplosObjects = Instantiate(ExplosionPreb, new Vector3(StartPos.x, StartPos.y, StartPos.z), Quaternion.identity);
            ExplosObjects.transform.Rotate(10, 10, 10);
            Rigidbody Exrigidby = ExplosObjects.GetComponent<Rigidbody>();

            Vector3 RandomDir = Random.insideUnitSphere * ParticleForce;
  
            Destroy(ExplosObjects, LifeTime);
        }

    }
    void BulletAction(Vector3 spawnPosition, Vector3 direction)
    {
 
        audioSource.clip = attackClip;
        audioSource.Play();
        //GameObject bullet = Instantiate(bulletObject, new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z), Quaternion.identity);
        var bullet = GoliatPool.GetObject();
        bullet.transform.position = spawnPosition;
        bullet.transform.forward = direction;
        bullet.transform.Rotate(new Vector3(1, 0, 0), 90.0f);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(direction * bulletSpeed, ForceMode.Force);
        StartCoroutine(ReleaseBulletAfterTime(bullet, 0.25f));
    }
    private IEnumerator ReleaseBulletAfterTime(EnemyBullet bullet, float time)
    {
        yield return new WaitForSeconds(time);
        bullet.ReleaseToPool();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PlayerBullet")) { 
            audioSource.clip = audioClip;
            audioSource.Play();
            particlePrefab.Play();
            hp -= 5;
            Debug.Log(hp);
        }
        
    }

    public void OnDyingAnimationEnd()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Dying Backwards")) 
        {
            float normalizedTime = stateInfo.normalizedTime; 
            if (normalizedTime >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
   
}
