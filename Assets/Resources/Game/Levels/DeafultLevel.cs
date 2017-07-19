﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultLevel : Level
{
    /// +++ MEMBERS +++ ///

    GameObject mParticleSystem;

    public UnityEngine.UI.Text highscore;

    GameObject enemy1;
    GameObject enemy2;
    GameObject enemy3;

    float timer = 0;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public DefaultLevel(string name) : base(name)
    {

        //Equip a wand.
        AttractorWand rightWand = rightHand.AddComponent<AttractorWand>();
        AttractorWand leftWand = leftHand.AddComponent<AttractorWand>();

        rightWand.rightHand = true;
        leftWand.rightHand = false;

        //Spawn enemies.
        SpawnEnemies();
    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {

    }

    /// <summary>
    /// Override update function.
    /// </summary>
    public override void Update()
    {
        if (VrInput.LeftGrip())
        {

            SpawnEnemies();
            timer = 0;

        }

        if ((enemy1 || enemy2 || enemy3))
            timer += Time.deltaTime;

    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {
        
    }

    void SpawnEnemies()
    {
        Object.Destroy(enemy1);
        Object.Destroy(enemy2);
        Object.Destroy(enemy3);

        //Spawn enemies.
        enemy1 = CreateGameObject("ENEMY1");
        enemy2 = CreateGameObject("ENEMY2");
        enemy3 = CreateGameObject("ENEMY3");

        enemy1.AddComponent<BasicEnemy>();
        enemy2.AddComponent<BasicEnemy>();
        enemy3.AddComponent<BasicEnemy>();

        enemy1.transform.position += Vector3.forward * 3 + Vector3.right * 3;
        enemy2.transform.position += Vector3.forward * 3 + Vector3.right * 0;
        enemy3.transform.position += Vector3.forward * 3 + Vector3.right * -3;

    }

    /// --- FUNCTIONS --- ///
}
