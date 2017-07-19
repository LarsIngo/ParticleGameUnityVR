﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugWand : MonoBehaviour
{

    public GameObject mEndAttractor;

    public float mPowerEndAttractor;
    public float mTimerEndAttractor = 0.1f;
    private float mTimerCurrentEndAttractor = 0.1f;

    public GameObject mParticleEmitter;

    public bool rightHand;
    public float mPowerAttractors;
    public float mNormalAttractorReboundDistance;

    private float timerSpeed = 1.0f;


    // Update is called once per frame
    void Update()
    {
        if (VrInput.controllersFound)
        {

            float trigger = 0;
            if (rightHand)
                trigger = VrInput.RightTrigger();
            else trigger = VrInput.LeftTrigger();



            
            if (trigger == 1.0f)
            {
                

                mParticleEmitter.transform.localPosition = new Vector3(Mathf.Cos(Time.time * timerSpeed), Mathf.Sin(Time.time * timerSpeed), 0.0f) * mNormalAttractorReboundDistance;


                mParticleEmitter.GetComponent<GPUParticleSystem>().Active = true;
                mParticleEmitter.GetComponent<GPUParticleAttractor>().Power = mPowerAttractors;
                mTimerCurrentEndAttractor = mTimerEndAttractor;

                mEndAttractor.GetComponent<GPUParticleAttractor>().Power = 0.0f;


                
            }
            else
            {

                mParticleEmitter.GetComponent<GPUParticleSystem>().Active = false;
                if (mTimerCurrentEndAttractor > 0.0f)
                {
                    mTimerCurrentEndAttractor -= Time.deltaTime;

                    mParticleEmitter.GetComponent<GPUParticleAttractor>().Power = 0.0f;
                    mEndAttractor.GetComponent<GPUParticleAttractor>().Power = mPowerEndAttractor;

                }
                else
                {
                    mEndAttractor.GetComponent<GPUParticleAttractor>().Power = 0.0f;
                }
            }
        }
        else
        {
            
            
        }

    }
}
