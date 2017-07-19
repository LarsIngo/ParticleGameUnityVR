using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugLevel : Level
{
    /// +++ MEMBERS +++ ///
    public UnityEngine.UI.Text highscore;

    GameObject enemy;
    private Vector3 prevPos;



    bool swap = false;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public VatsugLevel(string name) : base(name)
    {
        enemy = this.CreateGameObject("TheOneAndOnlyVatsug");
        enemy.AddComponent<Vatsug>();

        GameObject boat = this.CreateGameObject("ImOnABoat");
        boat.AddComponent<Boat>();
        

        StageInfo stageInfo = new StageInfo(2,0, Hub.STATE.VATSUG);
        stageInfo.mName = "Vatsug Level";

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
        prevPos = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Override update function.
    /// </summary>
    public override void Update()
    {
        
        /*
        float x = Mathf.Tan(Time.time / 8.0f);
        float y = Mathf.Cos(Time.time) * 3.0f;
        float z = Mathf.Sin(Time.time / 2) * 4.0f;// Mathf.Cos(Time.time / 3) * 3.0f;
     

        Vector3 newPos = new Vector3(x, y, z);

        enemy.transform.position = prevPos;
        enemy.transform.LookAt(newPos);
        prevPos = newPos;*/
    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {

    }
    
    /// --- FUNCTIONS --- ///
}
