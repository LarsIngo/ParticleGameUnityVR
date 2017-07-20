using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugLevel : Level
{
    /// +++ MEMBERS +++ ///
    public UnityEngine.UI.Text highscore;

    GameObject enemy;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public VatsugLevel(string name) : base(name)
    {
        enemy = this.CreateGameObject("TheOneAndOnlyVatsug");
        Factory.CreateVatsug(this, enemy.transform);
        enemy.AddComponent<Vatsug>();

        GameObject boat = this.CreateGameObject("ImOnABoat");
        boat.AddComponent<Boat>();
        

        StageInfo stageInfo = new StageInfo(2,0, Hub.STATE.VATSUG);
        stageInfo.mName = "Vatsug Level";

        Hub.Instance.mStageInfoList.Add(stageInfo);

        //Equip a wand.
        //GameObject rightWand = Factory.CreateVatsugWand(this, 90.0f, 35.0f, 5.0f, 15.0f, true);
        GameObject leftWand = Factory.CreateAttractorWand(this, 20, false);

        //rightWand.transform.parent = rightHand.transform;
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
        
    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {

    }
    
    /// --- FUNCTIONS --- ///
}
