using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringForce : ForceGenerator
{
    public Transform other = null;

    public float springConstant;

    public float restLength;

    public override void UpdateForce(Particle2D particle)
    {
        if (!other) return;
        
        Vector2 direction = transform.position - other.position;
        float distance = direction.magnitude - restLength;
        Vector2 force = direction.normalized * (-(springConstant) * distance);
        particle.AddForce(force);
    }
}
