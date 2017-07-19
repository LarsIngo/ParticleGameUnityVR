using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : Level
{
    /// +++ MEMBERS +++ ///

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public MenuLevel(string name) : base(name)
    {

        //Equip a wand.
        MenuWand rightWand = rightHand.AddComponent<MenuWand>();
        rightWand.rightHand = true;

        for(int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            GameObject screen = CreateGameObject("Screen");
            screen.AddComponent<StageScreen>();
            
            screen.transform.position += Vector3.forward + Vector3.right * (i + 0.1f);

        }

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
