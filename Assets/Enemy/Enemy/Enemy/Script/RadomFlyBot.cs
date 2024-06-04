using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadomFlyBot : MonoBehaviour
{
    public GameObject RadomObject1Prefab;
    public int numObjects = 50;
    public float areaSize = 21;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numObjects; i++)
        {
            float xPos = Random.Range(0, areaSize);
            float zPos = Random.Range(0, areaSize);
            Vector3 spawnPosition = new Vector3(xPos * i / 2, 10f, zPos * i / 2);
            Instantiate(RadomObject1Prefab, spawnPosition, Quaternion.identity);
        }
    }
}
