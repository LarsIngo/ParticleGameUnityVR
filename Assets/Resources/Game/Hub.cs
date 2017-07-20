using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class containing global data and manages levels.
/// </summary>
public class Hub
{
    /// +++ MEMBERS +++ ///

    /// <summary>
    /// Game states.
    /// </summary>
    public enum STATE
    {
        NONE = 0,
        MENU,
        ATTRACTOR_LVL_1,
        ATTRACTOR_LVL_2,
        VATSUG,
        SKULL,
        PARTICLETEST,
        DEFAULT
    };

    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static Hub mInstance;

    /// <summary>
    /// Dictionary of levels.
    /// </summary>
    private Dictionary<string, Level> mLevelDic;

    /// <summary>
    /// Current state of game.
    /// Default: STATE.NONE
    /// </summary>
    private STATE mCurrentState = STATE.NONE;

    /// <summary>
    /// Last state of game.
    /// Default: STATE.NONE
    /// </summary>
    private STATE mLastState = STATE.NONE;

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
        // Create dictionary.
        mLevelDic = new Dictionary<string, Level>();
        mStageInfoList = new List<StageInfo>();

        backgroundMusic = new GameObject("BACKGROUNDMUSIC");
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
        mLevelDic.Clear();
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
            }
            return mInstance;
        }
    }

    /// <summary>
    /// Update current state.
    /// </summary>
    /// <param name="state">State to be set.</param>
    public void SetState(STATE state)
    {
        mLastState = mCurrentState;
        mCurrentState = state;
    }

    /// <summary>
    /// Current state of game.
    /// </summary>
    public STATE CurrentState
    {
        get
        {
            return mCurrentState;
        }
    }

    /// <summary>
    /// Last state of game.
    /// </summary>
    public STATE LastState
    {
        get
        {
            return mLastState;
        }
    }

    /// <summary>
    /// Get current active level.
    /// </summary>
    public Level ActiveLevel
    {
        get
        {
            return mActiveLevel;
        }
    }

    /// <summary>
    /// Main camera of game. Initialized at StartUp.
    /// </summary>
    /// <param name="level">Level to add.</param>
    public void AddLevel(Level level)
    {
        Debug.Assert(level != null);
        Debug.Assert(!mLevelDic.ContainsKey(level.Name));

        mLevelDic[level.Name] = level;
    }

    /// <summary>
    /// Set active level. Deactive other levels.
    /// </summary>
    /// <param name="level">Level to set active.</param>
    public void SetActiveLevel(Level level)
    {
        if (mActiveLevel != null) mActiveLevel.SetActive(false);

        level.SetActive(true);

        mActiveLevel = level;
    }

    /// --- FUNCTIONS --- ///
}
