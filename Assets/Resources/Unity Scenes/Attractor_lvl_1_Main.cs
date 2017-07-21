﻿using System.Collections;
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
                if (mStageInfo.Score > mTimer)
                    mStageInfo.SetScore(mTimer);
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
