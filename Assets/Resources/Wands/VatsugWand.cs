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

    public GPUParticleSystem mParticles = null;

    public bool rightHand;
    public float mPowerAttractors;
    public float mNormalAttractorReboundDistance;

    


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

                float TwoPIdivNrAttractors = Mathf.PI * 2 / mNrOfAttractors;
                for (int i = 0; i < mNrOfAttractors; ++i)
                {
                    mAttractors[i].transform.localPosition = new Vector3(Mathf.Cos(TwoPIdivNrAttractors * i), 0.0f, Mathf.Sin(TwoPIdivNrAttractors * i)).normalized * 5 * Mathf.Sin(Time.time);

                    mAttractors[i].AddComponent<GPUParticleAttractor>().Power = mPowerAttractors;
                }

                mParticles.Active = true;
                mTimerCurrentEndAttractor = mTimerEndAttractor;

                mEndAttractor.GetComponent<GPUParticleAttractor>().Power = 0.0f;


                
            }
            else
            {
                for (int i = 0; i < mNrOfAttractors; ++i)
                  mAttractors[i].GetComponent<GPUParticleAttractor>().Power = 0.0f;

                mParticles.Active = false;
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
            /*
            if (Input.GetKey(KeyCode.Space))
            {

                attractor.Power = 20;
                system.Active = false;

            }
            else
            {

                attractor.Power = 0;
                system.Active = true;

            }
            */
        }

    }
}
