using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesignScrip : MonoBehaviour
{
    public GameObject RotationCube;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        RotationCube.transform.Rotate(2,0,2);
    }
}
