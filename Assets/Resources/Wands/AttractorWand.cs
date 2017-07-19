using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorWand : MonoBehaviour {

    public GPUParticleAttractor attractor;
    public GPUParticleSystem system;

    public bool rightHand;
    public float power;

    // Update is called once per frame
    void Update () {

        if (VrInput.controllersFound)
        {

            float trigger = 0;
            if (rightHand)
                trigger = VrInput.RightTrigger();
            else trigger = VrInput.LeftTrigger();

            
            attractor.Power = power * trigger;

            if (trigger == 1.0f)
                system.Active = true;
            else system.Active = false;

        }
        else
        {

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

        }

    }
}
