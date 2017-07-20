using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullLevel : Level
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
    public SkullLevel(string name) : base(name)
    {

        StageInfo stageInfo = new StageInfo(4, 0, Hub.STATE.SKULL);
        stageInfo.mName = "Skull Level";

        Hub.Instance.mStageInfoList.Add(stageInfo);

        //Equip a wand.
        GameObject rightWand = Factory.CreateAttractorWand(this, 20, true);
        GameObject leftWand = Factory.CreateAttractorWand(this, 20, false);

        rightWand.transform.parent = rightHand.transform;
        leftWand.transform.parent = leftHand.transform;

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
        Object.DestroyImmediate(enemy1);
        Object.DestroyImmediate(enemy2);
        Object.DestroyImmediate(enemy3);

        //Spawn enemies.
        enemy1 = Factory.CreateEnemySkull(this);
        enemy2 = Factory.CreateEnemySkull(this);
        enemy3 = Factory.CreateEnemySkull(this);

        enemy1.transform.position += Vector3.forward * 3 + Vector3.right * 3;
        enemy2.transform.position += Vector3.forward * 3 + Vector3.right * 0;
        enemy3.transform.position += Vector3.forward * 3 + Vector3.right * -3;

        enemy1.transform.LookAt(Vector3.zero);
        enemy2.transform.LookAt(Vector3.zero);
        enemy3.transform.LookAt(Vector3.zero);

    }

    /// --- FUNCTIONS --- ///
}
