using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : Level
{
    /// +++ MEMBERS +++ ///

    /// <summary>
    /// Storing all the created scenes.
    /// </summary>
    private List<GameObject> mScreenList;

    /// <summary>
    /// Storing all the created scenes.
    /// </summary>
    private UnityEngine.UI.Text mStarText;

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

        GameObject timerText = Factory.CreateWorldText(this, Hub.Instance.stars.ToString(), Color.white);

        timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        timerText.transform.localScale *= 100;

        mStarText = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();

    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {

        mStarText.text = Hub.Instance.stars.ToString();

        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            GameObject screen = Factory.CreateStageScreen(this, Hub.Instance.mStageInfoList[i]);

            screen.transform.position += Vector3.forward * 2 + Vector3.up * 1.5f;

            //Move it to the world location
            screen.transform.position += Vector3.right * Hub.Instance.mStageInfoList[i].world + Vector3.right * Hub.Instance.mStageInfoList[i].world * 0.1f;

            //Move it to the stage location
            screen.transform.position -= Vector3.up * Hub.Instance.mStageInfoList[i].stage + Vector3.up * Hub.Instance.mStageInfoList[i].stage * 0.5f;

            mScreenList.Add(screen);

        }

    }

    /// <summary>
    /// Override update function.
    /// </summary>
    public override void Update()
    {

        if (VrInput.RightGripPressed())
        {

            for (int i = 0; i < mScreenList.Count; i++)
            {

                mScreenList[i].transform.position -= VrInput.deltaRight.x * Vector3.right * 5 + VrInput.deltaRight.y * Vector3.up * 5;
    
            }

        }

    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {

        for (int i = 0; i < mScreenList.Count; i++)
            Object.Destroy(mScreenList[i]);

        mScreenList.Clear();

    }

    /// --- FUNCTIONS --- ///
}
