using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug2 : MonoBehaviour
{

    private Vector3 prevPos;
    private float sounddelay;
    private AudioSource sound;

    // Use this for initialization
    void Start()
    {
        gameObject.transform.position = new Vector3(100, 100, 100);
        prevPos = new Vector3(0, 0, 0);
        sound = gameObject.GetComponentInChildren<AudioSource>();
        Debug.Assert(sound);
        sounddelay = Random.Range(4.0f, 9.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time;
        
        Vector3 newPos = new Vector3(Mathf.Cos(t / 2) * 3.0f, Mathf.Sin(t * 2) * 0.3f +  2.5f, Mathf.Sin(t) * 3.0f);

        gameObject.transform.position = prevPos;
        gameObject.transform.LookAt(newPos);
        prevPos = newPos;

        if (!gameObject.GetComponentInChildren<Transform>())
        {
            Destroy(gameObject);
        }
        sounddelay -= Time.deltaTime;
        if (sounddelay <= 0.0f)
        {
            sounddelay = Random.Range(4.0f, 9.0f);
            sound.Play();
        }
    }
}
