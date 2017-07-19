using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : Level
{
    /// +++ MEMBERS +++ ///

    /// <summary>
    /// Level currently active.
    /// </summary>
    private List<GameObject> mScreenList;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public MenuLevel(string name) : base(name)
    {

        //Equip a wand.
        GameObject menuWand = Factory.CreateMenuWand(this, true);
        menuWand.transform.parent = rightHand.transform;

        mScreenList = new List<GameObject>();

    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {

        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            GameObject screen = Factory.CreateStageScreen(this, Hub.Instance.mStageInfoList[i]);

            screen.transform.position += Vector3.forward + Vector3.right * i * 0.1f;

            mScreenList.Add(screen);

        }

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

        for (int i = 0; i < mScreenList.Count; i++)
            Object.Destroy(mScreenList[i]);

        for (int i = 0; i < mScreenList.Count; i++)
            mScreenList.RemoveAt(i);

    }

    /// --- FUNCTIONS --- ///
}
