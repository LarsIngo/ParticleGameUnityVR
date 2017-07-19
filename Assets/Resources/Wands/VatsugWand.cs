using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugWand : MonoBehaviour
{
    
    public uint mNrOfAttractors;

    public GameObject[] mAttractors;
    

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



            mParticleEmitter.transform.localPosition = new Vector3(Mathf.Cos(Time.deltaTime), 0.0f, Mathf.Sin(Time.deltaTime)).normalized * mNormalAttractorReboundDistance;

            if (trigger == 1.0f)
            {

                float TwoPIdivNrAttractors = Mathf.PI * 2 / mNrOfAttractors;


                mParticleEmitter.transform.localPosition = new Vector3(Mathf.Cos(Time.deltaTime * timerSpeed), Mathf.Sin(Time.deltaTime * timerSpeed), 0.0f) * mNormalAttractorReboundDistance;


                mParticleEmitter.GetComponent<GPUParticleSystem>().Active = true;
                mTimerCurrentEndAttractor = mTimerEndAttractor;

                mEndAttractor.GetComponent<GPUParticleAttractor>().Power = 0.0f;


                
            }
            else
            {
                for (int i = 0; i < mNrOfAttractors; ++i)
                  mAttractors[i].GetComponent<GPUParticleAttractor>().Power = 0.0f;

                mParticleEmitter.GetComponent<GPUParticleSystem>().Active = false;
                if (mTimerCurrentEndAttractor > 0.0f)
                {
                    mTimerCurrentEndAttractor -= Time.deltaTime;
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
