using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    /// <summary>
    /// Factor to scales health to number of paritlces.
    /// </summary>
    public static float HEALTH_FACTOR = 1.0f;

    float mHealthStart = 1000;
    float mHealthCurrent = 1000;

    /// <summary>
    /// Start health of entity.
    /// Default: 1000
    /// </summary>
    public float HealthStart { get { return mHealthStart / HEALTH_FACTOR; } set { mHealthStart = value * HEALTH_FACTOR; mHealthCurrent = value * HEALTH_FACTOR; } }

    /// <summary>
    /// Current health of entity.
    /// </summary>
    public float HealthCurrent { get { return mHealthCurrent / HEALTH_FACTOR; } }

    void Start ()
    {

	}

	void Update ()
    {
        float healthFactor = (float)mHealthCurrent / mHealthStart;

        Renderer renderer = GetComponent<Renderer>();
        Debug.Assert(renderer);
        renderer.material.color = Color.Lerp(Color.green, Color.red, 1 - healthFactor);

        GPUParticleSphereCollider collider = GetComponent<GPUParticleSphereCollider>();
        Debug.Assert(collider);

        mHealthCurrent -= collider.CollisionsThisFrame;

        if (mHealthCurrent <= 0)
        {
            Destroy(gameObject);
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Debug.Assert(meshFilter);
            Factory.CreateMichaelBayEffect(meshFilter != null ? meshFilter.mesh : null, transform, renderer != null ? renderer.material.color : Color.red);

            GPUParticleSphereCollider.SKIPFRAME = true;
        }
    }
}
