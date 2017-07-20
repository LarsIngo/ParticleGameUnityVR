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
        // SKYBOX.
        Material skyboxMat = new Material(Shader.Find("RenderFX/Skybox"));
        Debug.Assert(skyboxMat);
        string skyboxName = "Stars01";
        Texture2D front = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/frontImage");
        Texture2D back = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/backImage");
        Texture2D left = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/leftImage");
        Texture2D right = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/rightImage");
        Texture2D up = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/upImage");
        Texture2D down = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/downImage");
        Debug.Assert(front);
        Debug.Assert(back);
        Debug.Assert(left);
        Debug.Assert(right);
        Debug.Assert(up);
        Debug.Assert(down);
        skyboxMat.SetTexture("_FrontTex", front);
        skyboxMat.SetTexture("_BackTex", back);
        skyboxMat.SetTexture("_LeftTex", left);
        skyboxMat.SetTexture("_RightTex", right);
        skyboxMat.SetTexture("_UpTex", up);
        skyboxMat.SetTexture("_DownTex", down);

        Skybox skybox = Camera.main.GetComponent<Skybox>();
        if (skybox == null)
        {
            skybox = Camera.main.gameObject.AddComponent<Skybox>();
            Camera.main.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        }
        skybox.material = skyboxMat;
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
