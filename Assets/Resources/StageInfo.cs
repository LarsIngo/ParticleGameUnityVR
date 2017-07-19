using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo  {

    static public int count = 0;

    public int world;
    public int stage;

    public int starRequirement;
    public bool locked;

    public string name;
    public string thumbnail;
    public Hub.STATE stageState;

    private float mScore = 9999;
    public float Score { get { return mScore; } }

    public void SetScore(float newScore)
    {

        if (mScore < newScore)
            return;

        int starsEarned = 0;

        if (mScore < bronze)
            starsEarned--;
        if (mScore < silver)
            starsEarned--;
        if (mScore < gold)
            starsEarned--;

        if (newScore < gold)
        {
            mScore = newScore;
            Hub.Instance.stars += 3;
        }
        else if (newScore < silver)
        {
            mScore = newScore;
            Hub.Instance.stars += 2;
        }
        else if (newScore < bronze)
        {
            mScore = newScore;
            Hub.Instance.stars += 1;
        }

    }

    public float bronze;
    public float silver;
    public float gold;

    public StageInfo()
    {

        name = "Stage:" + count;
        thumbnail = "Textures/Default";
        stageState = Hub.STATE.DEFAULT;

        count++;

    }

}
