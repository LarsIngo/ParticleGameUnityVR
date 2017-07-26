using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor_lvl_9_Main : MonoBehaviour
{

    /// +++ MEMBERS +++ ///

    StageInfo mStageInfo;

    float mTimer = 0;
    UnityEngine.UI.Text mTimerDisplay;

    List<GameObject> mEnemyList = new List<GameObject>();
    List<float> timeOffset = new List<float>();
    const int nrOfEnemies = 5;
    const float fac = Mathf.PI * 2 / nrOfEnemies;


    bool mFirstEnterDone;


    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            if (Hub.Instance.mStageInfoList[i].mSceneName == "Attractor_lvl_9")
                mStageInfo = Hub.Instance.mStageInfoList[i];

        }

        // RESET.
        mFirstEnterDone = true;

        //Equip a wand.
        GameObject rightWand = Factory.CreateAttractorWand(20, true);
        GameObject leftWand = Factory.CreateAttractorWand(20, false);

        // TIMER.
        GameObject timerText = Factory.CreateWorldText("Highscore", Color.white);
        timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        timerText.transform.localScale *= 100;
        mTimerDisplay = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        mTimer = 0;

        // Highscore.
        GameObject Highscore = Factory.CreateWorldText("Highscore:" + mStageInfo.highscore, Color.white);
        Highscore.transform.position += Vector3.forward * 100 + Vector3.up * 30;
        Highscore.transform.localScale *= 20;

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

    float endTimer = 0;
    // Update is called once per frame
    void Update()
    {

        // Check if level completed.
        bool levelDone = true;

        for (int i = 0; i < mEnemyList.Count; ++i)
        {

            if (mEnemyList[i])
            {
                levelDone = false;

                mEnemyList[i].transform.position = new Vector3(Mathf.Sin(Time.time + timeOffset[i]) * 6, Mathf.Tan(Time.time * 3 + timeOffset[i]), 4.0f);
            }
        }


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
                float score = 100 - mTimer;
                if (mStageInfo.Score < score)
                    mStageInfo.SetScore(score);

            }

            endTimer += Time.deltaTime;
            if (endTimer > 6)
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        }

        // Update time to display.
        mTimerDisplay.text = Mathf.Max(100 - mTimer, 0).ToString("0.00");


        // Check input to leave scene.
        if (VrInput.Menu() || Input.GetKeyDown(KeyCode.Escape))
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        }

    }

    void SpawnEnemies()
    {
        for (int i = 0; i < nrOfEnemies; ++i)
        {
            GameObject enemy = Factory.CreateBasicEnemy(Vector3.forward * 3, 1000);
            mEnemyList.Add(enemy);
            timeOffset.Add(fac * i);

        }

        return;
    }

}
