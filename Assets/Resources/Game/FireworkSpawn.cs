﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkSpawn : MonoBehaviour
{
    private float mTimer = 0.0f;

    public float mFrequency = 5.0f; 

    private void Awake()
    {

    }

    private void Update()
    {
        mTimer += Time.deltaTime;

        int count = (int)(mFrequency * mTimer);

        if (count == 0) return;

        mTimer -= count * 1.0f / mFrequency;

        for (int i = 0; i < count; ++i)
        {
            int sign = (((int)(Random.Range(0,100))) % 2) * 2 - 1;
            float min = 3;
            float max = 5;
            Factory.CreateFireworkHead(new Vector3(sign * Random.Range(min, max), sign * Random.Range(min, max), sign * Random.Range(min, max)));
            Factory.CreateFireworkTail(new Vector3(sign * Random.Range(min, max), sign * Random.Range(min, max), sign * Random.Range(min, max)));
        }
    }

}
