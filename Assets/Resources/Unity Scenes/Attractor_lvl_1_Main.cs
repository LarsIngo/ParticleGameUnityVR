using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor_lvl_1_Main : MonoBehaviour {

    /// +++ MEMBERS +++ ///

    StageInfo mStageInfo;

    float mTimer = 0;
    UnityEngine.UI.Text mTimerDisplay;

    List<GameObject> mEnemyList = new List<GameObject>();

    bool mFirstEnterDone;

    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start () {

        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            if (Hub.Instance.mStageInfoList[i].mSceneName == "Attractor_lvl_1")
                mStageInfo = Hub.Instance.mStageInfoList[i];

        }

        // RESET.
        mFirstEnterDone = true;

        // WAND.
        GameObject rightWand = Factory.CreateAttractorWand(20, true);

        // TIMER.
        GameObject timerText = Factory.CreateWorldText("Highscore", Color.white);
        timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        timerText.transform.localScale *= 100;
        mTimerDisplay = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        mTimer = 0;

        // ENEMIES.
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

    // Update is called once per frame
    void Update () {

        // Check if level completed.
        bool levelDone = true;
        for (int i = 0; i < mEnemyList.Count && levelDone; ++i)
            if (mEnemyList[i])
                levelDone = false;

        if (!levelDone)
            mTimer += Time.deltaTime;
        else
        {
            if (mFirstEnterDone)
            {
                mFirstEnterDone = false;

                // Celebration! :D :D :D
                Factory.CreateCelebration();

                // Update score.
                float score = 300 - mTimer * 30;
                if (mStageInfo.Score < score)
                    mStageInfo.SetScore(score);
            }
        }

        // Update time to display.
        mTimerDisplay.text = mTimer.ToString("0.00");


        // Check input to leave scene.
        if (VrInput.Menu() || Input.GetKeyDown(KeyCode.Escape))
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        }

    }

    void SpawnEnemies()
    {
        // ENEMIES.
        mEnemyList.Add(Factory.CreateBasicEnemy(Vector3.forward * 3 + Vector3.right * 3, 250));
        mEnemyList.Add(Factory.CreateBasicEnemy(Vector3.forward * 3 + Vector3.right * 0, 250));
        mEnemyList.Add(Factory.CreateBasicEnemy(Vector3.forward * 3 + Vector3.right * -3, 250));
    }

}
