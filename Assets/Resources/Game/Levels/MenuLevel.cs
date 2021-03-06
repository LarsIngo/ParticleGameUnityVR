﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MenuLevel : Level
//{
//    /// +++ MEMBERS +++ ///

//    /// <summary>
//    /// Storing all the created scenes.
//    /// </summary>
//    private List<GameObject> mScreenList;

//    /// <summary>
//    /// Storing all the created scenes.
//    /// </summary>
//    private UnityEngine.UI.Text mStarText;

//    /// --- MEMBERS --- ///


//    /// +++ FUNCTIONS +++ ///

//    /// <summary>
//    /// Constructor.
//    /// </summary>
//    /// <param name="name">Name of level, must be unique.</param>
//    public MenuLevel(string name) : base(name)
//    {



//    }


//    /// <summary>
//    /// Override awake function.
//    /// </summary>
//    public override void Awake()
//    {

//        mStarText.text = Hub.Instance.stars.ToString();

//        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
//        {

//            GameObject screen = Factory.CreateStageScreen(Hub.Instance.mStageInfoList[i]);

//            screen.transform.position += Vector3.forward * 2 + Vector3.up * 1.5f;

//            //Move it to the world location
//            screen.transform.position += Vector3.right * Hub.Instance.mStageInfoList[i].mWorld + Vector3.right * Hub.Instance.mStageInfoList[i].mWorld * 0.1f;

//            //Move it to the stage location
//            screen.transform.position -= Vector3.up * Hub.Instance.mStageInfoList[i].mStage + Vector3.up * Hub.Instance.mStageInfoList[i].mStage * 0.5f;

//            mScreenList.Add(screen);

//        }

//        // SKYBOX.
//        Material skyboxMat = new Material(Shader.Find("RenderFX/Skybox"));
//        Debug.Assert(skyboxMat);
//        string skyboxName = "Stars01";
//        Texture2D front = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/frontImage");
//        Texture2D back = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/backImage");
//        Texture2D left = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/leftImage");
//        Texture2D right = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/rightImage");
//        Texture2D up = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/upImage");
//        Texture2D down = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/downImage");
//        Debug.Assert(front);
//        Debug.Assert(back);
//        Debug.Assert(left);
//        Debug.Assert(right);
//        Debug.Assert(up);
//        Debug.Assert(down);
//        skyboxMat.SetTexture("_FrontTex", front);
//        skyboxMat.SetTexture("_BackTex", back);
//        skyboxMat.SetTexture("_LeftTex", left);
//        skyboxMat.SetTexture("_RightTex", right);
//        skyboxMat.SetTexture("_UpTex", up);
//        skyboxMat.SetTexture("_DownTex", down);

//        Skybox skybox = Camera.main.GetComponent<Skybox>();
//        if (skybox == null)
//        {
//            skybox = Camera.main.gameObject.AddComponent<Skybox>();
//            Camera.main.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
//        }
//        skybox.material = skyboxMat;

//    }

//    /// <summary>
//    /// Override update function.
//    /// </summary>
//    public override void Update()
//    {

//        if (VrInput.RightGripPressed())
//        {

//            for (int i = 0; i < mScreenList.Count; i++)
//            {

//                mScreenList[i].transform.position -= VrInput.deltaRight.x * Vector3.right * 5 + VrInput.deltaRight.y * Vector3.up * 5;
    
//            }

//        }

//    }

//    /// <summary>
//    /// Override sleep function.
//    /// </summary>
//    public override void Sleep()
//    {

//        for (int i = 0; i < mScreenList.Count; i++)
//            Object.Destroy(mScreenList[i]);

//        mScreenList.Clear();

//    }

//    /// --- FUNCTIONS --- ///
//}
