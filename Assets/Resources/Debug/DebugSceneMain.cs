using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSceneMain : MonoBehaviour
{

    //GameObject emitter;
    //https://gist.github.com/mre/1392067#file-bitonic_sort-cu-L50

    const int MAX_NUM_VALS = 4;
    const int PARTICLE_COUNT = 4;
    const int THREADS = MAX_NUM_VALS; // 2^9
    const int BLOCKS = 1; // 2^15

    struct SORTELEMENT
    {
        public float mValue;
        public int mIndex;
    }

    SORTELEMENT[] dev_values = new SORTELEMENT[MAX_NUM_VALS];
    Vector3[] particles = new Vector3[MAX_NUM_VALS];

    void CPU()
    {
        // Fill array
        for (int i = 0; i < MAX_NUM_VALS; ++i)
        {
            if (i < PARTICLE_COUNT)
            {
                particles[i].x = Random.Range(0, 100);
                particles[i].y = Random.Range(0, 100);
                particles[i].z = i;//Random.Range(0, 100);
                dev_values[i].mValue = particles[i].z;
                dev_values[i].mIndex = i;
            }
        }

        // Print array
        Debug.Log("PREINIT");
        for (int i = 0; i < MAX_NUM_VALS; ++i)
        {
            Debug.Log(i + " : " + dev_values[i].mValue);
        }

        // DISPATCH INIT
        for (int tID = 0; tID < MAX_NUM_VALS; ++tID)
        {
            if (tID >= PARTICLE_COUNT)
            {
                Debug.Log("INIT");
                dev_values[tID].mValue = float.MaxValue;
            }
        }

        //// Print array
        //Debug.Log("POSTINIT");
        //for (int i = 0; i < MAX_NUM_VALS; ++i)
        //{
        //    Debug.Log(i + " : " + dev_values[i].mValue);
        //}

        // Sort
        /* Major step */
        for (int k = 2; k <= MAX_NUM_VALS; k <<= 1)
        {
            /* Minor step */
            for (int j = k >> 1; j > 0; j = j >> 1)
            {
                //Sort(j, k);
                // DISPATCH SORT
                for (int tID = 0; tID < MAX_NUM_VALS; ++tID)
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
                            if (dev_values[i].mValue > dev_values[ixj].mValue)
                            {
                                // exchange(i,ixj);
                                SORTELEMENT temp = dev_values[i];
                                dev_values[i] = dev_values[ixj];
                                dev_values[ixj] = temp;
                            }
                        }
                        else if ((i & k) != 0)
                        {
                            // Sort descending
                            if (dev_values[i].mValue < dev_values[ixj].mValue)
                            {
                                // exchange(i,ixj);
                                SORTELEMENT temp = dev_values[i];
                                dev_values[i] = dev_values[ixj];
                                dev_values[ixj] = temp;
                            }
                        }
                    }
                }

                Debug.Log("------");
                for (int i = 0; i < PARTICLE_COUNT; ++i)
                {
                    Debug.Log(i + " : " + dev_values[i].mValue);
                }

            }
        }

        // Print array
        Debug.Log("Sorted");
        for (int i = 0; i < PARTICLE_COUNT; ++i)
        {
            //Debug.Log(i + " : " + particles[dev_values[i].mIndex]);
            Debug.Log(i + " : " + dev_values[i].mValue);
        }
    }

    void Start()
    {
        //for (int x = 0; x < 4; ++x)
        //{
        //    for (int y = 0; y < 4; ++y)
        //    {
        //        Debug.Log("(" + x + "," + y + ") ");
        //        Debug.Log("x & y" + (x & y));
        //        Debug.Log("x ^ y" + (x ^ y));
        //    }
        //}

        GPU();
    }
	
	void Update ()
    {


    }

    void GPU()
    {
        GameObject emitter = new GameObject("emitter");
        GPUParticleSystem system = emitter.AddComponent<GPUParticleSystem>();
        system.EmittParticleLifeTime = 30.0f;
        system.EmittFrequency = 500.0f;

        GameObject att = new GameObject("attractor");
        att.transform.position = new Vector3(0,0,5);
        GPUParticleAttractor attractor = att.AddComponent<GPUParticleAttractor>();
    }

}
