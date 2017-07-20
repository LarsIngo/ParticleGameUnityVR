using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultLevel : Level
{
    /// +++ MEMBERS +++ ///

    GameObject mParticleSystem;

    GameObject mHMD;
    public GameObject mLeftController;
    public GameObject mRightController;

    GameObject enemy1;
    GameObject enemy2;
    GameObject enemy3;

    float timer = 0;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public DefaultLevel(string name) : base(name)
    {

        StageInfo stageInfo = new StageInfo(0,0, Hub.STATE.DEFAULT);
        stageInfo.mName = "Default Level";

        Hub.Instance.mStageInfoList.Add(stageInfo);

    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {

        //Equip a wand.
        GameObject rightWand = Factory.CreateAttractorWand(this, 20, true);
        GameObject leftWand = Factory.CreateAttractorWand(this, 20, false);

        //Spawn enemies.
        SpawnEnemies();

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
        if (VrInput.LeftGrip())
        {

            SpawnEnemies();
            timer = 0;

        }

        if ((enemy1 || enemy2 || enemy3))
            timer += Time.deltaTime;

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
        enemy1 = Factory.CreateBasicEnemy(this);
        enemy2 = Factory.CreateBasicEnemy(this);
        enemy3 = Factory.CreateBasicEnemy(this);

        enemy1.transform.position += Vector3.forward * 3 + Vector3.right * 3;
        enemy2.transform.position += Vector3.forward * 3 + Vector3.right * 0;
        enemy3.transform.position += Vector3.forward * 3 + Vector3.right * -3;

    }

    /// --- FUNCTIONS --- ///
}
