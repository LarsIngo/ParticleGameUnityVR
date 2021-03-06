﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedBack : MonoBehaviour
{

    float mTimeSinceLastHit = 0.0f;
    float mLastFrameHealth = 0.0f;
    float mDamageTaken = 0.0f;
    float mResetTime = 2.0f;

    enum STATE
    {
        NONE = 0,
        COOL,
        AWESOME,
        OMFG
    }

    STATE mLastState = STATE.NONE;
    STATE mCurrentState = STATE.NONE;

	void Start ()
    {

        Health health = GetComponent<Health>();
        Debug.Assert(health, "Make sure to add health componenet before feedback.");

        mLastFrameHealth = health.HealthStart;
    }
	
	void Update ()
    {
        mTimeSinceLastHit += Time.deltaTime;

        Health health = GetComponent<Health>();
        Debug.Assert(health, "Make sure to add health componenet to objects with feedback.");


        if (health.HealthCurrent != mLastFrameHealth)
        {
            // HIT!
            mTimeSinceLastHit = 0.0f;

            Debug.Assert(mLastFrameHealth > health.HealthCurrent);
            mDamageTaken += mLastFrameHealth - health.HealthCurrent;

            // Update state.
            if (mDamageTaken > 1600)
            {
                mCurrentState = STATE.OMFG;
            }
            else if (mDamageTaken > 800)
            {
                mCurrentState = STATE.AWESOME;
            }
            else if (mDamageTaken > 200)
            {
                mCurrentState = STATE.COOL;
            }
        }
        else if (mTimeSinceLastHit > mResetTime)
        {
            mDamageTaken = 0;
            mLastState = mCurrentState = STATE.NONE;
        }

        // Check state.
        if (mCurrentState > mLastState)
        {
            switch (mCurrentState)
            {
                case STATE.NONE:
                    Debug.Log("NONE");
                    break;

                case STATE.COOL:
                    Factory.CreateFeedbackText("COOL", Color.red, transform.position + Vector3.up, new Vector3(-1, 1, 0.1f));
                    break;

                case STATE.AWESOME:
                    Factory.CreateFeedbackText("AWESOME", Color.red, transform.position + Vector3.up, new Vector3(0, 2, 0.1f));
                    break;

                case STATE.OMFG:
                    Factory.CreateFeedbackText("O M G!!", Color.red, transform.position + Vector3.up, new Vector3(1, 3, 0.1f));
                    break;

                default:
                    Debug.Log("NO ASSIGNED STATE");
                    break;
            }
            mLastState = mCurrentState;
        }

        mLastFrameHealth = health.HealthCurrent;
    }

}
