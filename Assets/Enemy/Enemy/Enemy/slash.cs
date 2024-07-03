using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slash : MonoBehaviour
{
    public ParticleSystem slashPrefab;
    ParticleSystem slashInstance;
    public Transform inittrans;

    private void Update()
    {
        if(slashInstance!=null)
        {
            slashInstance.transform.position = inittrans.position;
        }
    }
    public void OnSlashEffect()
    {

        slashInstance = Instantiate(slashPrefab, inittrans.position, inittrans.rotation);
        slashInstance.Play();
        Destroy(slashInstance.gameObject, 1.5f);
    }
}
