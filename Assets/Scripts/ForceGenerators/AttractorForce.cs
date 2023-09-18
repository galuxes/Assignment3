using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttractorForce : ForceGenerator
{
    public Vector3 targetPos;
    public float power;

    public override void UpdateForce(Particle2D particle)
    {
        Vector2 vector = (targetPos - transform.position);
        float invMagSqr = 1/vector.sqrMagnitude;
        particle.AddForce(power * invMagSqr * vector.normalized);
    }
}
