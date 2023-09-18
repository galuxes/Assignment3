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
        Vector2 direction = other.position - transform.position;
        Vector2 force = -1 * (springConstant * restLength) * direction;
        particle.AddForce(force);
    }
}
