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
    private GameObject mLevelGO;

    /// <summary>
    /// Right hand game object.
    /// </summary>
    protected GameObject rightHand;

    /// <summary>
    /// Left hand game object.
    /// </summary>
    protected GameObject leftHand;


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
        
        mLevelGO = new GameObject(name);

        mSpawnSystem = this.CreateGameObject("spawnSystem" + name);
        mSpawnSystem.AddComponent<SpawnSystem>();

        mLevelGO.SetActive(false);

        Hub.Instance.AddLevel(this);

        rightHand = CreateGameObject(name + ":Right Hand");
        rightHand.AddComponent<MirrorHandMovement>().rightHand = true;
        leftHand = CreateGameObject(name + ":Left Hand");
        leftHand.AddComponent<MirrorHandMovement>().rightHand = false;

    }

    /// <summary>
    /// Create new game object to level.
    /// </summary>
    /// <param name="value">Name of game object, must be unique.</param>
    /// <returns>Returns new game object.</returns>
    public GameObject CreateGameObject(string name)
    {
        Debug.Assert(!GameObject.Find(name));

        GameObject go = new GameObject(name);

        go.transform.SetParent(mLevelGO.transform);

        return go;
    }

    /// <summary>
    /// Kills all children.
    /// </summary>
    /// <param name="value">Name of game object, must be unique.</param>
    /// <returns>Returns new game object.</returns>
    public void KillAll()
    {
        KillChildren(mLevelGO);
    }

    private static void KillChildren(GameObject parent)
    {
        Debug.Log("KILL");
        for (int i = 0; i < parent.transform.childCount; ++i)
        {
            KillChildren(parent.transform.GetChild(i).gameObject);
            Object.DestroyImmediate(parent.transform.GetChild(i).gameObject);
        }
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
        get { return mLevelGO.name; }
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
