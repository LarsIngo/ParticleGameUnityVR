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
    private Level mVatsugLevel;
    private Level mMenuLevel;
    private Level mSkullLevel;

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
        //mVatsugLevel = new VatsugLevel("LEVEL:VATSUG");
        mSkullLevel = new SkullLevel("LEVEL:SKULL");

        Hub.Instance.SetState(Hub.STATE.MENU);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            Hub.Instance.SetState(Hub.STATE.MENU);
        if (Input.GetKeyDown(KeyCode.F2))
            Hub.Instance.SetState(Hub.STATE.ATTRACTOR_LVL_1);
        if (Input.GetKeyDown(KeyCode.F3))
            Hub.Instance.SetState(Hub.STATE.ATTRACTOR_LVL_2);
        if (Input.GetKeyDown(KeyCode.F4))
        //    Hub.Instance.SetState(Hub.STATE.VATSUG);
        //if (Input.GetKeyDown(KeyCode.F5))
            Hub.Instance.SetState(Hub.STATE.DEFAULT);
        if (Input.GetKeyDown(KeyCode.F6))
            Hub.Instance.SetState(Hub.STATE.SKULL);

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

            //case Hub.STATE.VATSUG:
            //    mCurrentLevel = mVatsugLevel;
            //    break;

            case Hub.STATE.DEFAULT:
                mCurrentLevel = mDeafultLevel;
                break;

            case Hub.STATE.SKULL:
                mCurrentLevel = mSkullLevel;
                break;

            default: Debug.Log("WARNING: No assigned STATE"); break;
        }

        // Switch active level.
        if (Hub.Instance.ActiveLevel != mCurrentLevel)
        {
            if (Hub.Instance.ActiveLevel != null)
                Hub.Instance.ActiveLevel.Sleep();

            // Kill all living particles.
            GPUParticleSystem.KillAllParticles();
            Debug.Log(Hub.Instance.CurrentState + " KILL PARTICLES");

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
