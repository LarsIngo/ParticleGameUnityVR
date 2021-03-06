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
            float min = -10;
            float max = 10;
            Factory.CreateFireworkHead(new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(3, max)));
        }
    }

}