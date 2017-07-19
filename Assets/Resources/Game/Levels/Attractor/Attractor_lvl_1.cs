using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor_lvl_1 : Level
{
    /// +++ MEMBERS +++ ///

    StageInfo stageInfo;

    GameObject enemy1;
    GameObject enemy2;
    GameObject enemy3;

    UnityEngine.UI.Text highscore;

    float timer = 0;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public Attractor_lvl_1(string name) : base(name)
    {

        stageInfo = new StageInfo();
        stageInfo.name = "My first wand!";
        stageInfo.thumbnail = "Textures/Attractor_lvl_1";
        stageInfo.stageState = Hub.STATE.ATTRACTOR_LVL_1;
        stageInfo.world = 1;
        stageInfo.stage = 0;
        stageInfo.bronze = 30;
        stageInfo.silver = 15;
        stageInfo.gold = 10;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        //Equip a wand.
        GameObject rightWand = Factory.CreateAttractorWand(this, 20, true);

        rightWand.transform.parent = rightHand.transform;

        GameObject timerText = Factory.CreateWorldText(this, "Highscore", Color.white);

        timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        timerText.transform.localScale *= 100;

        highscore = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();


    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {
        //Spawn enemies.
        SpawnEnemies();
        timer = 0;
    }

    /// <summary>
    /// Override update function.
    /// </summary>
    public override void Update()
    {

        if ((enemy1 || enemy2 || enemy3))
            timer += Time.deltaTime;
        else
        {

            if (stageInfo.Score > timer)
                stageInfo.SetScore(timer);

        }
        highscore.text = timer.ToString("0.00");

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

        enemy1.AddComponent<BasicEnemy>().Health = 500;
        enemy2.AddComponent<BasicEnemy>().Health = 500;
        enemy3.AddComponent<BasicEnemy>().Health = 500;

        enemy1.transform.position += Vector3.forward * 3 + Vector3.right * 3;
        enemy2.transform.position += Vector3.forward * 3 + Vector3.right * 0;
        enemy3.transform.position += Vector3.forward * 3 + Vector3.right * -3;

    }

    /// --- FUNCTIONS --- ///
}
