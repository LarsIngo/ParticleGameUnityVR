using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug2 : MonoBehaviour
{

    private Vector3 prevPos;
    private float sounddelay;
    private AudioSource sound;

    private Vector3 mul;
    private Vector3 offs;
    private float timeOffset;
    private int algoritm;

    // Use this for initialization
    void Start()
    {
        gameObject.transform.position = new Vector3(100, 100, 100);
        prevPos = new Vector3(0, 0, 0);
        sound = gameObject.GetComponentInChildren<AudioSource>();
        Debug.Assert(sound);
        sounddelay = Random.Range(1.0f, 15.0f);
       

        mul = new Vector3(Random.Range(2.0f, 4.0f), Random.Range(0.1f, 1.0f), Random.Range(2.0f, 4.0f));
        offs = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(2.0f, 3.0f), Random.Range(-1.0f, 1.0f));

        timeOffset = Random.Range(0.0f, Mathf.PI * 2 - 0.01f);

        algoritm = Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time + timeOffset;
        
        Vector3 newPos = new Vector3(0,0,0);
        if (algoritm == 0)
            newPos = new Vector3(Mathf.Cos(t / 2) * mul.x, Mathf.Sin(t * 2) * mul.y, Mathf.Sin(t) * mul.z) + offs;
        else if (algoritm == 1)
        {
            newPos = new Vector3(Mathf.Sin(t) * mul.x, Mathf.Sin(t * 2) * mul.y, Mathf.Cos(t / 3) * mul.z) + offs;
        }
        else if (algoritm == 2)
        {
            newPos = new Vector3(Mathf.Sin(t / 4) * mul.x, Mathf.Sin(t * 3) * mul.y, Mathf.Cos(t) * mul.z) + offs;
        }

        gameObject.transform.position = prevPos;
        gameObject.transform.LookAt(newPos);
        prevPos = newPos;

        sounddelay -= Time.deltaTime;
        if (sounddelay <= 0.0f && sound)
        {
            sounddelay = Random.Range(6.0f, 16.0f);
            sound.Play();
        }
    }
}
