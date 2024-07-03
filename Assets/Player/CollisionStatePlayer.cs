using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStatePlayer : MonoBehaviour
{
    #region Variables
    RaycastHit hit;
    private GameObject FireStartPosition;
    public GameObject BulletObject;
    public GameObject ExplosionPreb;
    public GameObject longExplosionPreb;
    private GameObject HeadPosition;
    private float DieParticleForce = 10;

    private float ParticleForce = 5;
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
        HeadPosition = GameObject.Find("mixamorig:Head");
    }

    void Update()
    {
        Attacked();
       
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
            
        }
    
    }
    
    #endregion CustomMethods
}
