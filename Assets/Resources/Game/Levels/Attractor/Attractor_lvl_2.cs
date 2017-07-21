using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor_lvl_2 : Level
{
    /// +++ MEMBERS +++ ///

    StageInfo mStageInfo;

    float mTimer = 0;
    UnityEngine.UI.Text mTimerDisplay;

    List<GameObject> mEnemyList = new List<GameObject>();

    bool mFirstEnterDone;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public Attractor_lvl_2(string name) : base(name)
    {

        mStageInfo = new StageInfo(1, 0, Hub.STATE.ATTRACTOR_LVL_1);
        mStageInfo.mName = "My first wand!";
        mStageInfo.mThumbnail = "Textures/Attractor_lvl_1";
        mStageInfo.mBronze = 30;
        mStageInfo.mSilver = 15;
        mStageInfo.mGold = 10;

        Hub.Instance.mStageInfoList.Add(mStageInfo);

    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {
        // RESET.
        mFirstEnterDone = true;

        // WAND.
        GameObject rightWand = Factory.CreateAttractorWand(this, 20, true);
        GameObject leftWand = Factory.CreateAttractorWand(this, 20, false);

        // TIMER.
        GameObject timerText = Factory.CreateWorldText(this, "Highscore", Color.white);
        timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        timerText.transform.localScale *= 100;
        mTimerDisplay = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        mTimer = 0;

        // ENEMIES.
        SpawnEnemies();
    }

    /// <summary>
    /// Override update function.
    /// </summary>
    public override void Update()
    {
        // Chech if level completed.
        bool levelDone = true;
        for (int i = 0; i < mEnemyList.Count && levelDone; ++i)
            if (mEnemyList[i])
                levelDone = false;

        if (!levelDone)
            mTimer += Time.deltaTime;
        else
        {

            // ENTER ONCE WHEN LEVEL IS COMPLETE.
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

    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {
        
    }

    void SpawnEnemies()
    {
        // ENEMIES.
        GameObject enemyBlueprint = Factory.CreateBasicEnemy(this);
        enemyBlueprint.GetComponent<Health>().HealthStart = 1000;

        enemyBlueprint.transform.position = Vector3.forward * 3 + Vector3.right * 3;
        mEnemyList.Add(Object.Instantiate(enemyBlueprint, enemyBlueprint.transform.parent));

        enemyBlueprint.transform.position = Vector3.forward * 3 + Vector3.right * 0;
        mEnemyList.Add(Object.Instantiate(enemyBlueprint, enemyBlueprint.transform.parent));

        enemyBlueprint.transform.position = Vector3.forward * 3 + Vector3.right * -3;
        mEnemyList.Add(Object.Instantiate(enemyBlueprint, enemyBlueprint.transform.parent));

        Object.DestroyImmediate(enemyBlueprint);
    }

    /// --- FUNCTIONS --- ///
}
