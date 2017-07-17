using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mParticleSystem;

    GameObject mHMD;
    public GameObject mLeftController;
    public GameObject mRightController;

    GameObject enemy1;
    GameObject enemy2;
    GameObject enemy3;

    private void Start ()
    {
        mHMD = GameObject.Find("Camera (head)");

        if (mLeftController == null)
        {
            mLeftController = new GameObject("STATIC LEFT CONTROLLER");
            mLeftController.transform.position = new Vector3(-3,0,5);
        }
        if (mRightController == null)
        {
            mRightController = new GameObject("STATIC RIGHT CONTROLLER");
            mRightController.transform.position = new Vector3(3, 0, 5);
        }

        //Equip a wand.
        mRightController.AddComponent<AttractorWand>();

        //Spawn enemies.
        SpawnEnemies();

    }

    private void Update()
    {

        if (!(enemy1 || enemy2 || enemy3) && VrInput.LeftGrip())
            SpawnEnemies();

    }

    private void OnDestroy()
    {

    }

    void SpawnEnemies()
    {

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

}
