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

    //Particle Test level.
    private Level mParticleTestLevel;

    /// --- SCENES --- ///

    private void Start ()
    {


        
    }

    private void Update()
    {




    }

    private void OnDestroy()
    {
        Hub.Instance.ShutDown();
    }

}
