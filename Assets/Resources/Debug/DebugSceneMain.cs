﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSceneMain : MonoBehaviour
{

    //GameObject emitter;

    const int THREADS = 2; // 2^9
    const int BLOCKS = 2; // 2^15
    const int NUM_VALS = THREADS * BLOCKS;
    float[] dev_values = new float[NUM_VALS];

    //Vector3[] valueArray = new Vector3[NUM_VALS];

    void Start()
    {
        //emitter = new GameObject("emitter");
        //emitter.transform.position = new Vector3(0,0,5);
        //GPUParticleSystem system = emitter.AddComponent<GPUParticleSystem>();
        //system.EmittFrequency = 10.0f;
        //system.EmittParticleLifeTime = 5.0f;

        //GameObject attractor = new GameObject("attractor");
        //attractor.transform.position = new Vector3(0, 0, 0);
        //GPUParticleAttractor a = attractor.AddComponent<GPUParticleAttractor>();

        //GameObject vf = new GameObject("vector field");
        //vf.transform.position = new Vector3(5, 0, 5);
        //GPUParticleVectorField b = vf.AddComponent<GPUParticleVectorField>();

        //GameObject collider0 = new GameObject("collider0");
        //collider0.transform.position = new Vector3(5, 0, 0);
        //collider0.AddComponent<GPUParticleSphereCollider>();

        //GameObject collider1 = new GameObject("collider1");
        //collider1.transform.position = new Vector3(-5, 0, 0);
        //collider1.AddComponent<GPUParticleSphereCollider>();

        // Fill array
        for (int i = 0; i < NUM_VALS; ++i)
        {
            dev_values[i] = Random.Range(0, 100);
        }

        //// Print array
        //Debug.Log("");
        //for (int i = 0; i < NUM_VALS; ++i)
        //{
        //    Debug.Log(i + " : " + dev_values[i]);
        //}

        // Sort
        /* Major step */
        for (int k = 2; k <= NUM_VALS; k <<= 1)
        {
            /* Minor step */
            for (int j = k >> 1; j > 0; j = j >> 1)
            {
                //bitonic_sort_step <<< blocks, threads >>> (dev_values, j, k);
                Sort(j, k);
            }
        }

        // Print array
        Debug.Log("Sorted");
        for (int i = 0; i < NUM_VALS; ++i)
        {
            Debug.Log(i + " : " + dev_values[i]);
        }

    }

    void Sort(int j, int k)
    {
        for (int tID = 0; tID < NUM_VALS; ++tID)
        {
            int i, ixj; // Sorting partners: i and ixj
            i = tID;
            ixj = i ^ j;

            // The threads with the lowest ids sort the array.
            if ((ixj) > i)
            {
                if ((i & k) == 0)
                {
                    // Sort ascending
                    if (dev_values[i] > dev_values[ixj])
                    {
                        // exchange(i,ixj);
                        float temp = dev_values[i];
                        dev_values[i] = dev_values[ixj];
                        dev_values[ixj] = temp;
                    }
                }
                if ((i & k) != 0)
                {
                    // Sort descending
                    if (dev_values[i] < dev_values[ixj])
                    {
                        // exchange(i,ixj);
                        float temp = dev_values[i];
                        dev_values[i] = dev_values[ixj];
                        dev_values[ixj] = temp;
                    }
                }
            }
        }
    }
	
	void Update ()
    {


    }

}
