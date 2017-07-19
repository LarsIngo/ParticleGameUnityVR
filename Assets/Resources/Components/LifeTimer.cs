using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimer : MonoBehaviour {

    private float mLifeTime = float.MaxValue;

    /// <summary>
    /// Time left to live for this gameobject
    /// Default: Very large number
    /// </summary>
    public float LifeTime { get { return mLifeTime; } set { mLifeTime = value; } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mLifeTime -= Hub.Instance.DeltaTime;

        if (mLifeTime < 0.0f)
        {
            Destroy(gameObject);
        }
	}
}
