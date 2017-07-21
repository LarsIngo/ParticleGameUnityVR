using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo  {

    static public int count = 0;

    public int mWorld;
    public int mStage;

    public string mSceneName;

    public int mStarRequirement;
    public bool mLocked;

    public string mName;
    public string mThumbnail;

    private float mScore;
    public float Score { get { return mScore; } }
    public void SetScore(float newScore)
    {

        if (mScore < newScore)
            return;

        int starsEarned = 0;

        if (mScore >= mBronze)
            starsEarned--;
        if (mScore >= mSilver)
            starsEarned--;
        if (mScore >= mGold)
            starsEarned--;

        if (newScore >= mGold)
        {
            mScore = newScore;
            Hub.Instance.stars += 3 + starsEarned;
        }
        else if (newScore >= mSilver)
        {
            mScore = newScore;
            Hub.Instance.stars += 2 + starsEarned;
        }
        else if (newScore >= mBronze)
        {
            mScore = newScore;
            Hub.Instance.stars += 1 + starsEarned;
        }

    }

    public float mBronze;
    public float mSilver;
    public float mGold;

    public StageInfo(int world, int stage, string SceneName)
    {

        mWorld = world;
        mStage = stage;

        mSceneName = SceneName;

        mStarRequirement = 0;
        mLocked = false;

        mName = "Stage:" + count;
        mThumbnail = "Textures/Default";

        mScore = -1;

        mBronze = 50;
        mSilver = 30;
        mGold = 20;

        count++;

    }

}
