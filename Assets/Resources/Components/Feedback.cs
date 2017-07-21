using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedBack : MonoBehaviour
{

    float mTimeSinceLastHit = 0.0f;
    float mLastFrameHealth = 0.0f;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        Health health = GetComponent<Health>();
        Debug.Assert(health, "Make sure to add health componenet to objects with feedback.");


        //if ()

        mLastFrameHealth = health.HealthCurrent;
    }

}
