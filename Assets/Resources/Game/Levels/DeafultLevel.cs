using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeafultLevel : Level
{
    /// +++ MEMBERS +++ ///

    GameObject mParticleSystem;

    GameObject mHMD;
    public GameObject mLeftController;
    public GameObject mRightController;

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
    public DeafultLevel(string name) : base(name)
    {
        mHMD = GameObject.Find("Camera (head)");

        if (mLeftController == null)
        {
            mLeftController = new GameObject("STATIC LEFT CONTROLLER");
            mLeftController.transform.position = new Vector3(-3, 0, 5);
        }
        if (mRightController == null)
        {
            mRightController = new GameObject("STATIC RIGHT CONTROLLER");
            mRightController.transform.position = new Vector3(3, 0, 5);
        }

        //Equip a wand.
        AttractorWand rightWand = mRightController.AddComponent<AttractorWand>();
        AttractorWand leftWand = mLeftController.AddComponent<AttractorWand>();

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
            timer += Hub.Instance.DeltaTime;

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
        enemy1 = new GameObject("ENEMY1");
        enemy2 = new GameObject("ENEMY2");
        enemy3 = new GameObject("ENEMY3");

        enemy1.AddComponent<BasicEnemy>();
        enemy2.AddComponent<BasicEnemy>();
        enemy3.AddComponent<BasicEnemy>();

        enemy1.transform.position += Vector3.forward * 3 + Vector3.right * 3;
        enemy2.transform.position += Vector3.forward * 3 + Vector3.right * 0;
        enemy3.transform.position += Vector3.forward * 3 + Vector3.right * -3;

    }

    /// --- FUNCTIONS --- ///
}
