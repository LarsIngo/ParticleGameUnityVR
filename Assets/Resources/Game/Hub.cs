using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class containing global data and manages levels.
/// </summary>
public class Hub
{
    /// +++ MEMBERS +++ ///

    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static Hub mInstance;

    /// <summary>
    /// Current active level.
    /// Default: null
    /// </summary>
    private Level mActiveLevel = null;

    /// <summary>
    /// Current active level.
    /// Default: null
    /// </summary>
    public List<StageInfo> mStageInfoList;

    /// <summary>
    /// Current active level.
    /// Default: null
    /// </summary>
    public int stars;

    public static GameObject backgroundMusic;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    private Hub() { }

    /// <summary>
    /// Call to initialize singleton.
    /// </summary>
    public void StartUp()
    {

        // Create stage list.
        mStageInfoList = new List<StageInfo>();

        GameObject backgroundMusic = new GameObject("BACKGROUNDMUSIC");
        Object.DontDestroyOnLoad(backgroundMusic);
        AudioSource audioSource = backgroundMusic.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("Music/MachinimaSound.com_-_The_Arcade");
        audioSource.loop = true;
        audioSource.Play();

    }

    /// <summary>
    /// Call to deinitialize singleton.
    /// </summary>
    public void ShutDown()
    {
        mStageInfoList.Clear();
    }

    /// <summary>
    /// Instance of singleton.
    /// </summary>
    public static Hub Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new Hub();
                mInstance.StartUp();
            }
            return mInstance;
        }
    }

}
