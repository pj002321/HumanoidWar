using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MineEnermy : MonoBehaviour
{
    #region Variables

    public LayerMask playerLayer;
    public float speed = 5f;
    public float radius = 10f;

    private float jumpTimer = 0.0f;
    private float jumpInterval = 2f;

    private GameObject ExplosObjects;
    public GameObject ExplosionPreb;
    public ParticleSystem particlePrefab;
    private float ParticleForce = 5;
    private float LifeTime = 0.5f;
    private AudioSource audioSource;
    public AudioClip audioClip;
    public Animator anim;
    private NavMeshAgent agent;
    private Slider slider;
    public int hp;
    private int maxhp = 100;
    private int hashChasing = Animator.StringToHash("IsChasing");
    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        slider= GetComponentInChildren<Slider>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        hp = maxhp;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsAlive)
        {
            slider.value = 0f;
           Destroy(gameObject, 0.5f);
        }
        else
        {
            slider.value = hp * 0.01f;
            Collider[] playerInRange = Physics.OverlapSphere(transform.position, radius, playerLayer);

            if (playerInRange.Length > 0)
            {
                agent.SetDestination(playerInRange[0].transform.position);
                anim.SetBool(hashChasing, true);
                Jump();
            }
            else
            {
                anim.SetBool(hashChasing, false);
            }

            Jump();
        }
    }

    private bool IsAlive => hp > 0;

    void Jump()
    {
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpInterval)
        {
            Rigidbody Minrigidb = GetComponent<Rigidbody>();
            Minrigidb.AddForce(Vector3.up * 6f, ForceMode.Impulse);
            jumpTimer = 0.0f;
        }
    }

    void ExplosionAction()
    {

        for (int i = 0; i < 60; i++)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PlayerBullet"))
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            particlePrefab.Play();
            hp -= 5;
        }
    }

}
