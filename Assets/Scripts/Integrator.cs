using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(Particle2D particle, float dt)
    {
        particle.transform.position += new Vector3(particle.velocity.x * dt, particle.velocity.y * dt);

	// TODO: YOUR CODE HERE
	// Determine acceleration
	// --- END TODO

        particle.velocity += particle.acceleration * dt;
        particle.velocity *= Mathf.Pow(particle.damping, dt);
    }
}
