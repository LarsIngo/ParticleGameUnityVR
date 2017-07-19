using System.Collections;
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
    private Level mMenuLevel;

    //Attractor levels.
    private Level mAttractor_lvl_1;
    private Level mAttractor_lvl_2;

    /// --- SCENES --- ///

    private void Start ()
    {
        Hub.Instance.StartUp();

        mDeafultLevel = new DefaultLevel("LEVEL:DEFAULT");
        mMenuLevel = new MenuLevel("LEVEL:MENU");
        mAttractor_lvl_1 = new Attractor_lvl_1("LEVEL:ATTRACTOR_LVL_1");
        mAttractor_lvl_2 = new Attractor_lvl_2("LEVEL:ATTRACTOR_LVL_2");
        Hub.Instance.SetState(Hub.STATE.MENU);
    }

    private void Update()
    {

        // Check state.
        switch (Hub.Instance.CurrentState)
        {
            case Hub.STATE.MENU:
                mCurrentLevel = mMenuLevel;
                break;

            case Hub.STATE.ATTRACTOR_LVL_1:
                mCurrentLevel = mAttractor_lvl_1;
                break;

            case Hub.STATE.ATTRACTOR_LVL_2:
                mCurrentLevel = mAttractor_lvl_2;
                break;

            case Hub.STATE.DEFAULT:
                mCurrentLevel = mDeafultLevel;
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

        if (VrInput.Menu())
            Hub.Instance.SetState(Hub.STATE.MENU);

    }

    private void OnDestroy()
    {
        Hub.Instance.ShutDown();
    }

}
