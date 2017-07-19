using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStretcher : MonoBehaviour
{

    float mTimer = 0.0f;

    public float mTimeInPhase = 0.0f;
    public float mTimeMainPhase = 0.5f;
    public float mTimeOutPhase = 0.5f;

    public float mTargetTimeScale = 0.1f;

	void Awake ()
    {
        Time.timeScale = mTargetTimeScale;
    }

	void Update ()
    {
        // Increment timer with realtime.
        mTimer += (Time.deltaTime / Time.timeScale);


        float totalTime = mTimeInPhase + mTimeMainPhase + mTimeOutPhase;

        if (mTimer < mTimeInPhase)
        {
            // IN.
            float factor = mTimer / mTimeInPhase; // [0,1]
            Time.timeScale = CustomMath.Slerp(1, mTargetTimeScale, factor); // [1, mTargetTimeScale]
        }
        else if (mTimer < mTimeInPhase + mTimeMainPhase)
        {
            // MAIN.
            Time.timeScale = mTargetTimeScale;
        }
        else
        {
            // OUT.
            float factor = Mathf.Clamp01((mTimer - mTimeInPhase - mTimeMainPhase) / mTimeOutPhase); // [0,1]
            Time.timeScale = Time.timeScale = CustomMath.Slerp(mTargetTimeScale, 1, factor); // [mTargetTimeScale, 1]
        }

        // Set pitch to match time scale.
        AudioSource[] audioSourceArray = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < audioSourceArray.GetLength(0); ++i)
            audioSourceArray[i].pitch = Time.timeScale;

        // When done, remove self.
        if (mTimer > totalTime)
        {
            Time.timeScale = 1.0f;
            Destroy(gameObject);
        }
    }
}
