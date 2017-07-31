using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedBack : MonoBehaviour
{

    float mTimeSinceLastHit = 0.0f;
    float mLastFrameHealth = 0.0f;
    float mDamageTaken = 0.0f;
    float mResetTime = 1.0f;

    enum STATE
    {
        NONE = 0,
        COOL,
        GOOD,
        AWESOME,
        INSANE,
        OMFG,
        CHEATER
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
            if (mDamageTaken > 3000)
            {
                mCurrentState = STATE.CHEATER;
            }
            else if (mDamageTaken > 1600)
            {
                mCurrentState = STATE.OMFG;
            }
            else if (mDamageTaken > 1300)
            {
                mCurrentState = STATE.INSANE;
            }
            else if (mDamageTaken > 900)
            {
                mCurrentState = STATE.AWESOME;
            }
            else if (mDamageTaken > 500)
            {
                mCurrentState = STATE.GOOD;
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
                    Factory.CreateFeedbackText("COOL", Color.green, transform.position + Vector3.up, new Vector3(-1, 1, 0.1f));
                    break;

                case STATE.GOOD:
                    Factory.CreateFeedbackText("GOOD", new Color(0.0f, 0.7f, 0.0f), transform.position + Vector3.up, new Vector3(-1, 1.5f, 0.2f));
                    break;

                case STATE.AWESOME:
                    Factory.CreateFeedbackText("AWESOME", Color.yellow, transform.position + Vector3.up, new Vector3(0, 2, 0.1f));
                    break;

                case STATE.INSANE:
                    Factory.CreateFeedbackText("INSANE!", new Color(1.0f, 0.5f, 0.1f), transform.position + Vector3.up, new Vector3(1, 3.5f, 0.1f));
                    break;

                case STATE.OMFG:
                    Factory.CreateFeedbackText("O M G!!", Color.red, transform.position + Vector3.up, new Vector3(1, 5.0f, 0.1f));
                    break;

                case STATE.CHEATER:
                    Factory.CreateFeedbackText("CHEATER?!!", new Color(1.0f, 0.0f, 1.0f), transform.position + Vector3.up * 3.0f, new Vector3(0.0f, 0.0f, 0.0f));
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
