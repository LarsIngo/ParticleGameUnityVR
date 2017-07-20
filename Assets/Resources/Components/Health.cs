using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    int mHealthStart = 1000;
    int mHealthCurrent = 1000;

    /// <summary>
    /// Start health of entity.
    /// Default: 1000
    /// </summary>
    public int HealthStart { get { return mHealthStart; } set { mHealthStart = value; mHealthCurrent = value; } }

    /// <summary>
    /// Current health of entity.
    /// </summary>
    public int HealthCurrent { get { return mHealthCurrent; } set { mHealthCurrent = value; } }

    void Start ()
    {

	}

	void Update ()
    {
        float healthFactor = mHealthCurrent / mHealthStart;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.Lerp(Color.green, Color.red, 1 - healthFactor);

        GPUParticleSphereCollider collider = GetComponent<GPUParticleSphereCollider>();
        Debug.Assert(collider);
        mHealthCurrent -= collider.CollisionsThisFrame;

        if (mHealthCurrent <= 0.0f)
        {
            Destroy(gameObject);
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Debug.Assert(meshFilter);
            Factory.CreateMichaelBayEffect(meshFilter != null ? meshFilter.mesh : null, transform, renderer != null ? renderer.material.color : Color.red);
        }
    }
}
