using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorWand : MonoBehaviour {

    public GPUParticleAttractor attractor;
    public GPUParticleVectorField vectorField;
    public GPUParticleSystem system;

    public bool rightHand;
    public float power;

    public float range = 1;

    // Update is called once per frame
    void Update () {

        if (VrInput.controllersFound)
        {

            float trigger = 0;
            if (rightHand)
                trigger = VrInput.RightTrigger();
            else trigger = VrInput.LeftTrigger();

            if (rightHand)
            {

                vectorField.Vector = -VrInput.deltaRight * 50;


            }
            else
            {

                vectorField.Vector = -VrInput.deltaLeft * 50;
                vectorField.Max = 0.3f + VrInput.deltaLeft.magnitude * range;

            }
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

            }
            else
            {

                attractor.Power = 0;

            }

            system.Active = true;

            if (rightHand)
                vectorField.Vector = -VrInput.deltaRight * 50;
            else vectorField.Vector = -VrInput.deltaLeft * 50;

        }

    }
}
