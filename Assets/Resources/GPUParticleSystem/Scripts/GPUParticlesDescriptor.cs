using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPUParticleDescriptor {

    /// +++ Internal Classes +++ ///

    public class LifetimePoints
    {

        List<Vector4> points;

        public Vector3 basePoint = Vector3.one;

        public LifetimePoints()
        {

            points = new List<Vector4>();

        }

        public void Add(Vector4 value)
        {

            //We make sure the W element is between 0 - 1.
            Debug.Assert(value.w <= 1);
            Debug.Assert(value.w >= 0);

            //We add the value and sort them
            points.Add(value);
            points = points.OrderBy(x => x.w).ToList();

        }

        public Vector4[] Get()
        {

            bool addStart = points.Count == 0 || points[0].w != 0;
            bool addEnd = points.Count == 0 || points[points.Count - 1].w != 1;

            int extraSize = 0;
            if (addStart)
                extraSize++;
            if (addEnd)
                extraSize++;

            Vector4[] array = new Vector4[points.Count + extraSize];

            if (addStart)
            {

                if(points.Count == 0)
                {

                    array[0] = basePoint;
                    array[0].w = 0;

                }
                else
                {

                    array[0] = points[0];
                    array[0].w = 0;

                }

            }
            if (addEnd)
            {

                if (points.Count == 0)
                {

                    array[0] = basePoint;
                    array[0].w = 1;

                }
                else
                {

                    array[array.Length - 1] = points[points.Count - 1];
                    array[array.Length - 1].w = 1;

                }


            }

            int start = addStart ? 1 : 0;
            int end = addEnd ? array.Length - 1 : array.Length;
            for (int i = start; i < end; i++)
            {

                array[i] = Vector4.Scale(new Vector4(basePoint.x, basePoint.y, basePoint.z, 1), points[i]);

            }

            return array;

        }

        public int Length()
        {

            bool addStart = points.Count == 0 || points[0].w != 0;
            bool addEnd = points.Count == 0 || points[points.Count - 1].w != 1;

            int extraSize = 0;
            if (addStart)
                extraSize++;
            if (addEnd)
                extraSize++;

            return points.Count + extraSize;

        }

    }

    /// --- Internal Classes --- ///

    /// +++ MEMBERS +++ ///

    private bool mOutOfDate = false;
    /// <summary>
    /// Is the descriptor out of date.
    /// Default: 10
    /// </summary>
    public bool OutOfDate { get { return mOutOfDate; } }

    // MESH.
    private Mesh mEmittMesh = null; private Mesh mNewEmittMesh = null;
    /// <summary>
    /// Type of mesh to emitt from.
    /// If set to null, particles are emitted at emitter position.
    /// Default: null.
    /// </summary>
    public Mesh EmittMesh { get { return mEmittMesh; } set { mNewEmittMesh = value; mOutOfDate = true; } }

    private float mEmittFrequency = 10.0f; private float mNewEmittFrequency = 10.0f;
    /// <summary>
    /// How many partices to emitt per second.
    /// Default: 10
    /// </summary>
    public float EmittFrequency { get { return mEmittFrequency; } set { mNewEmittFrequency = value; mOutOfDate = true; } }

    private float mLifetime = 6.0f; private float mNewLifetime = 6.0f;
    /// <summary>
    /// How long a particle live in seconds.
    /// Default: 6
    /// </summary>
    public float Lifetime { get { return mLifetime; } set { mNewLifetime = value; mOutOfDate = true; } }

    private bool mInheritVelocity = true;
    /// <summary>
    /// Whether emitted particles inherit velocity from emitter.
    /// Default: true
    /// </summary>
    public bool InheritVelocity { get { return mInheritVelocity; } set { mInheritVelocity = value; } }

    private Vector3 mConstantAcceleration = Vector3.zero;
    /// <summary>
    /// Constant acceleration applyed to particles.
    /// Default: 0,0,0
    /// </summary>
    public Vector3 ConstantAcceleration { get { return mConstantAcceleration; } set { mConstantAcceleration = value; } }

    private float mConstantDrag = 1.0f;
    /// <summary>
    /// Constant drag applyed to particles.
    /// Default: 1
    /// </summary>
    public float ConstantDrag { get { return mConstantDrag; } set { mConstantDrag = value; } }

    private Vector3 mInitialVelocity = Vector3.zero;
    /// <summary>
    /// Initial velocity of emitted particle.
    /// Default: 0,0,0
    /// </summary>
    public Vector3 InitialVelocity { get { return mInitialVelocity; } set { mInitialVelocity = value; } }

    private LifetimePoints mColorOverLifetime; private LifetimePoints mNewColorOverLifetime;
    /// <summary>
    /// Points describing how the color should change over its lifetime.
    /// </summary>
    public LifetimePoints ColorOverLifetime { get { return mColorOverLifetime; } set { mNewColorOverLifetime = value; mOutOfDate = true; } }

    private LifetimePoints mHaloOverLifetime; private LifetimePoints mNewHaloOverLifetime;
    /// <summary>
    /// Points describing how the color should change over its lifetime.
    /// </summary>
    public LifetimePoints HaloOverLifetime { get { return mHaloOverLifetime; } set { mNewHaloOverLifetime = value; mOutOfDate = true; } }

    private LifetimePoints mScaleOverLifetime; private LifetimePoints mNewScaleOverLifetime;
    /// <summary>
    /// Points describing how the color should change over its lifetime.
    /// </summary>
    public LifetimePoints ScaleOverLifetime { get { return mScaleOverLifetime; } set { mNewScaleOverLifetime = value; mOutOfDate = true; } }

    private LifetimePoints mOpacityOverLifetime; private LifetimePoints mNewOpacityOverLifetime;
    /// <summary>
    /// Points describing how the color should change over its lifetime.
    /// </summary>
    public LifetimePoints OpacityOverLifetime { get { return mOpacityOverLifetime; } set { mNewOpacityOverLifetime = value; mOutOfDate = true; } }

    /// --- MEMBERS --- ///

    public GPUParticleDescriptor()
    {

        mNewColorOverLifetime = new LifetimePoints();
        mNewHaloOverLifetime = new LifetimePoints();
        mNewScaleOverLifetime = new LifetimePoints();
        mNewOpacityOverLifetime = new LifetimePoints();

    }

    public void Update()
    {

        mEmittMesh = mNewEmittMesh;

        mEmittFrequency = mNewEmittFrequency;
        mLifetime = mNewLifetime;

        mColorOverLifetime = mNewColorOverLifetime;
        mHaloOverLifetime = mNewHaloOverLifetime;
        mScaleOverLifetime = mNewScaleOverLifetime;
        mOpacityOverLifetime = mNewOpacityOverLifetime;

        mOutOfDate = false;

    }

}
