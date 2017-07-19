using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugLevel : Level
{
    /// +++ MEMBERS +++ ///
    public UnityEngine.UI.Text highscore;

    GameObject enemy;

    float timer = 0;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public VatsugLevel(string name) : base(name)
    {
        enemy = Factory.CreateBasicEnemy(this, new Vector3(0, 0, 3.0f));

        StageInfo stageInfo = new StageInfo();
        stageInfo.name = "Vatsug Level";
        stageInfo.stageState = Hub.STATE.VATSUG;
       


        Hub.Instance.mStageInfoList.Add(stageInfo);

        //Equip a wand.
        GameObject rightWand = Factory.CreateVatsugWand(this, 90.0f, 35.0f, 5.0f, 15.0f, true);
        GameObject leftWand = Factory.CreateAttractorWand(this, 20, false);

        rightWand.transform.parent = rightHand.transform;
        leftWand.transform.parent = leftHand.transform;
        
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

        if (enemy)
            timer += Time.deltaTime;

        float x = Mathf.Tan(Time.time / 12.0f);
        float y = Mathf.Cos(Time.time) * 3.0f + 2.0f;
        float z = Mathf.Sin(Time.time / 3) * 3.0f + Mathf.Cos(Time.time / 3) * 3.0f;

        if (z < 3.0f && z > 3.0f && x < 3.0f && x > 3.0f && y < 3.9f)
        {
            y = 3.9f;
        }


        enemy.transform.position.Set(x, y, z);// = ;= new Vector3(Mathf.Tan(Time.time / 12.0f), Mathf.Cos(Time.time) * 5.0f * Mathf.Sin(Time.time) * 2, Mathf.Sin(Time.time / 3) * 3.0f + Mathf.Cos(Time.time / 3) * 3.0f);

    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {

    }
    
    /// --- FUNCTIONS --- ///
}
