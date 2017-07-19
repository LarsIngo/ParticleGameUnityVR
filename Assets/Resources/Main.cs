﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    /// +++ SCENES +++ ///

    /// <summary>
    /// Level currently active.
    /// </summary>
    private Level mCurrentLevel;

    // Levels.
    private Level mDeafultLevel;
    private Level mVatsugLevel;

    /// --- SCENES --- ///

    private void Start ()
    {
        Hub.Instance.StartUp();

        mDeafultLevel = new DefaultLevel("LEVEL:DEFAULT");
        mVatsugLevel = new VatsugLevel("LEVEL:VATSUG");

        Hub.Instance.SetState(Hub.STATE.VATSUG);
    }

    private void Update()
    {

        // Check state.
        switch (Hub.Instance.CurrentState)
        {
            case Hub.STATE.DEFAULT:
                mCurrentLevel = mDeafultLevel;
                break;
            case Hub.STATE.VATSUG:
                mCurrentLevel = mVatsugLevel;
                break;

            default: Debug.Log("WARNING: No assigned STATE"); break;
        }

        // Switch active level.
        if (Hub.Instance.ActiveLevel != mCurrentLevel)
        {
            if (Hub.Instance.ActiveLevel != null)
                Hub.Instance.ActiveLevel.Sleep();

            Hub.Instance.SetActiveLevel(mCurrentLevel);
            mCurrentLevel.Awake();

        }

        // Update level.
        mCurrentLevel.Update();
    }

    private void OnDestroy()
    {
        Hub.Instance.ShutDown();
    }

}
