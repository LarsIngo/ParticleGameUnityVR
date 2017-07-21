using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class contaning a collection of game objects.
/// </summary>
public abstract class Level
{
    /// +++ MEMBERS +++ ///

    /// <summary>
    /// Level game object.
    /// </summary>
    private GameObject mLevelGO = null;

    /// <summary>
    /// Name of level.
    /// </summary>
    private string mName = null;

    public GameObject mSpawnSystem;

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public Level(string name)
    {
        Debug.Assert(!GameObject.Find(name));

        mName = name;


    }

    /// <summary>
    /// Create new game object to level.
    /// </summary>
    /// <param name="value">Name of game object, must be unique.</param>
    /// <returns>Returns new game object.</returns>
    public GameObject CreateGameObject(string name)
    {

        if(mLevelGO == null)
        {

            mLevelGO = new GameObject("LEVEL:" + name);

        }

        Debug.Assert(!GameObject.Find(name));

        GameObject go = new GameObject(name);

        go.transform.SetParent(mLevelGO.transform);

        return go;
    }

    /// <summary>
    /// Kill level.
    /// </summary>
    public void Kill()
    {
        Object.DestroyImmediate(mLevelGO);
        mLevelGO = null;
    }

    /// <summary>
    /// Create level.
    /// </summary>
    public void Create()
    {

        mSpawnSystem = this.CreateGameObject("spawnSystem" + mName);
        mSpawnSystem.AddComponent<SpawnSystem>();

    }

    /// <summary>
    /// Set active state of level.
    /// </summary>
    /// <param name="value">Whether level should be active.</param>
    public void SetActive(bool value)
    {
        mLevelGO.SetActive(value);
    }

    /// <summary>
    /// Get active state of level.
    /// </summary>
    public bool Active
    {
        get { return mLevelGO.activeSelf; }
    }

    /// <summary>
    /// Get name of level.
    /// </summary>
    public string Name
    {
        get { return mName; }
    }

    /// <summary>
    /// Virtual awake function.
    /// </summary>
    public virtual void Awake()
    {

    }

    /// <summary>
    /// Virtual update function.
    /// </summary>
    public virtual void Update()
    {

    }

    /// <summary>
    /// Virtual sleep function.
    /// </summary>
    public virtual void Sleep()
    {

    }

    /// --- FUNCTIONS --- ///
}
