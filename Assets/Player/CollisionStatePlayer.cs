using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStatePlayer : MonoBehaviour
{
    #region Variables
    RaycastHit hit;
    private GameObject MyCamera;
    private GameObject FireStartPosition;
    public GameObject BulletObject;
    public GameObject ExplosionPreb;
    public GameObject longExplosionPreb;
    private GameObject HeadPosition;
    private float ParticleForce = 5;
    private float DieParticleForce = 10;

    private float LifeTime = 0.5f;
    private float longLifeTime = 0.9f;
    float raycastDistance = 100.0f;
    int HumanBotHP = 100;

    int MineBotHP = 200;
    int FlyBotHP = 20;
    #endregion Variables

    #region UnityMethods
    void Start()
    {
        FireStartPosition = GameObject.Find("Rifle_1");
        MyCamera = GameObject.Find("Camera");
        HeadPosition = GameObject.Find("mixamorig:Head");
    }

    void Update()
    {
        Attacked();
        ObjectDie();
    }
    #endregion UnityMethods

    #region CustomMethods
    void Attacked()
    {
        Vector3 startPosition = FireStartPosition.transform.position;

        if (Physics.Raycast(startPosition, FireStartPosition.transform.forward, out hit, raycastDistance))
        {
            string tag = hit.collider.tag;
            Vector3 collisionPosition = hit.collider.gameObject.transform.position;

            if (Input.GetMouseButton(0) && tag == "MineBot")
            {
                MineBotHP -= 15;
                UnderattackExplosionAction(new Vector3(collisionPosition.x, collisionPosition.y + 0.3f, collisionPosition.z));
            }
            else if (Input.GetMouseButton(0) && tag == "HumanBot")
            {
                HumanBotHP -= 10;
                UnderattackExplosionAction(new Vector3(collisionPosition.x, collisionPosition.y + 1.3f, collisionPosition.z));
            }
            else if (Input.GetMouseButton(1) && tag == "FlyBot")
            {
                FlyBotHP -= 20;
                UnderattackExplosionAction(new Vector3(collisionPosition.x, collisionPosition.y, collisionPosition.z));
            }
        }
    }

    void ObjectDie()
    {
        CheckAndHandleDeath(ref MineBotHP, 200, "MineBot");
        CheckAndHandleDeath(ref HumanBotHP, 100, "HumanBot");
        CheckAndHandleDeath(ref FlyBotHP, 20, "FlyBot");
    }

    void CheckAndHandleDeath(ref int botHP, int resetHP, string tag)
    {
        if (botHP <= 0)
        {
            Vector3 position = hit.collider.gameObject.transform.position;
            DieExplosionAction(position);
            Destroy(hit.collider.gameObject);
            botHP = resetHP;
        }
    }

    void UnderattackExplosionAction(Vector3 position)
    {
        CreateExplosions(position, ExplosionPreb, 15, ParticleForce, LifeTime, 10.0f, 2);
    }

    void DieExplosionAction(Vector3 position)
    {
        CreateExplosions(position, longExplosionPreb, 50, DieParticleForce, longLifeTime, -10.0f, 3);
    }

    void CreateExplosions(Vector3 position, GameObject prefab, int count, float force, float lifetime, float directionOffset, float forceMultiplier)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject explosionObject = Instantiate(prefab, position, Quaternion.identity);
            explosionObject.transform.Rotate(10, 10, 10);
            Rigidbody rb = explosionObject.GetComponent<Rigidbody>();
            Vector3 randomDir = Random.insideUnitSphere * force;
            randomDir.x += directionOffset;
            rb.AddForce(randomDir * forceMultiplier, ForceMode.Impulse);
            Destroy(explosionObject, lifetime);
        }
    }
    #endregion CustomMethods
}
