using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Factory {

    public static GameObject CreateMichaelBayEffect(Mesh mesh, Transform t)
    {
        GameObject michael = new GameObject();
        michael.transform.position = t.position;
        GeometryExplosion exp = michael.AddComponent<GeometryExplosion>();
        exp.Mesh = mesh;

        exp.Explode();

        return michael;
    }

}
