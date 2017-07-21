﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug : MonoBehaviour {

    private Vector3 prevPos;
    private AudioSource sound;

    const int nrOfClips = 4;
    private AudioClip[] clips;
    
    // Use this for initialization
    void Start () {
        gameObject.transform.position = new Vector3(100, 100, 100);
        prevPos = new Vector3(0, 0, 0);
        sound = gameObject.GetComponentInChildren<AudioSource>();
        Debug.Assert(sound);

        clips = new AudioClip[nrOfClips];

        
        for (int i = 1; i < nrOfClips + 1; ++i)
            clips[i - 1] = Resources.Load<AudioClip>("Samples/Vatsug/splish" + i);
    }
	
	// Update is called once per frame
	void Update () {
        
        Vector3 newPos = new Vector3(Mathf.Tan((Time.time + 3.0f) / 8.0f), 
            Mathf.Cos(Time.time + 3.0f) * 3.0f + 1.0f, Mathf.Sin((3.0f + Time.time) / 2) * 3.0f);

        gameObject.transform.position = prevPos;
        gameObject.transform.LookAt(newPos);
        prevPos = newPos;

        if (Mathf.Abs(gameObject.transform.position.y) <= 0.1f && sound)
        {
            sound.Stop();
            sound.clip = clips[Random.Range(1, nrOfClips) - 1];
            sound.Play();
        }
    }
}
