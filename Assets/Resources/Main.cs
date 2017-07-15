using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mGO;

    private void Start ()
    {
        mGO = new GameObject();
        mGO.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mGO.AddComponent<GPUParticleSystem>();
        //system.SetEmittFrequency(120.0f);

    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
