using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStretch : MonoBehaviour
{

    private float mTimer = 0.0f;

    private float mTimeInPhase = 0.5f;
    /// <summary>
    /// Time to get to target time scale.
    /// </summary>
    public float TimePrePhase { get { return mTimeInPhase; } set { mTimeInPhase = value; } }

    private float mTimeMainPhase = 0.5f;
    /// <summary>
    /// Time in target time scale.
    /// </summary>
    public float TimeMainPhase { get { return mTimeMainPhase; } set { mTimeMainPhase = value; } }

    private float mTimeOutPhase = 0.5f;
    /// <summary>
    /// Time to leave target time scale.
    /// </summary>
    public float TimePostPhase { get { return mTimeOutPhase; } set { mTimeOutPhase = value; } }

    private float mTargetTimeScale = 0.1f;
    /// <summary>
    /// Time scale target amount.
    /// </summary>
    public float TargetTimeScale { get { return mTargetTimeScale; } set { mTargetTimeScale = value; } }

    private float mCurrentTimeScale = 1.0f;

    private static Dictionary<TimerStretch, TimerStretch> sTimeStretchDictionary = null;

    private static bool mUpdateAudioSources;

    private static void UpdateAudioSources()
    {
        // Set pitch to match time scale.
        AudioSource[] audioSourceArray = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < audioSourceArray.GetLength(0); ++i)
            audioSourceArray[i].pitch = Time.timeScale;
    }

	void Awake ()
    {
        if (sTimeStretchDictionary == null)
            sTimeStretchDictionary = new Dictionary<TimerStretch, TimerStretch>();

        sTimeStretchDictionary[this] = this;
    }

	void Update ()
    {
        mUpdateAudioSources = true;

        // Increment timer with realtime.
        mTimer += (Time.deltaTime / Time.timeScale);

        float totalTime = mTimeInPhase + mTimeMainPhase + mTimeOutPhase;

        if (mTimer < mTimeInPhase)
        {
            // IN.
            float factor = mTimer / mTimeInPhase; // [0,1]
            mCurrentTimeScale = CustomMath.Slerp(1, mTargetTimeScale, factor); // [1, mTargetTimeScale]
        }
        else if (mTimer < mTimeInPhase + mTimeMainPhase)
        {
            // MAIN.
            mCurrentTimeScale = mTargetTimeScale;
        }
        else
        {
            // OUT.
            float factor = Mathf.Clamp01((mTimer - mTimeInPhase - mTimeMainPhase) / mTimeOutPhase); // [0,1]
            mCurrentTimeScale = CustomMath.Slerp(mTargetTimeScale, 1, factor); // [mTargetTimeScale, 1]
        }
    }

    private void LateUpdate()
    {
        if (mUpdateAudioSources)
        {
            mUpdateAudioSources = false;
            UpdateAudioSources();

            float minTimer = float.MaxValue;
            float timeScale = 1.0f;
            // Find the latest timestretch and set it as time scale.
            foreach (KeyValuePair<TimerStretch, TimerStretch> it in sTimeStretchDictionary)
            {
                if (it.Value.mTimer < minTimer)
                {
                    minTimer = it.Value.mTimer;
                    timeScale = it.Value.mCurrentTimeScale;
                }
            }

            Time.timeScale = timeScale;
        }
    }

    private void OnDestroy()
    {
        sTimeStretchDictionary.Remove(this);

        if (sTimeStretchDictionary.Count == 0)
        {
            sTimeStretchDictionary = null;
            Time.timeScale = 1.0f;
        }
    }

}
