using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStretcher : MonoBehaviour
{

    float mTimer = 0.0f;

    float mTimeIn = 0.0f;
    float mTimeMain = 0.5f;
    float mTimeOut = 0.5f;

    float mTimeScale = 0.1f;

	void Awake ()
    {
        Time.timeScale = mTimeScale;
    }

	void Update ()
    {
        mTimer += (Time.deltaTime / Time.timeScale);

        float totalTime = mTimeIn + mTimeMain + mTimeOut;

        if (mTimer < mTimeIn)
        {
            // IN.
            float lerpFactor = mTimer / mTimeIn; // [0,1]
            float slerpFactor = (1.0f - Mathf.Cos(lerpFactor * Mathf.PI)) / 2.0f; //[0,1]
            float timeScale = mTimeScale + slerpFactor * (1.0f - mTimeScale);
            Time.timeScale = Mathf.Clamp(1.0f - timeScale, mTimeScale, 1);
        }
        else if (mTimer < mTimeIn + mTimeMain)
        {
            // MAIN.
            Time.timeScale = mTimeScale;
        }
        else
        {
            // OUT.
            float lerpFactor = Mathf.Clamp01((mTimer - mTimeIn - mTimeMain) / mTimeOut); // [0,1]
            float slerpFactor = (1.0f - Mathf.Cos(lerpFactor * Mathf.PI)) / 2.0f; // [0,1]

            float timeScale = mTimeScale + slerpFactor * (1.0f - mTimeScale);
            Time.timeScale = Mathf.Clamp(timeScale, mTimeScale, 1); ;
        }

        AudioSource[] audioSourceArray = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < audioSourceArray.GetLength(0); ++i)
            audioSourceArray[i].pitch = Time.timeScale;

        if (mTimer > totalTime)
        {
            Time.timeScale = 1.0f;
            Destroy(gameObject);
        }
    }
}
