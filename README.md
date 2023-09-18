# Starting Project

Start your project by creating a **private** template from this
repository: <https://github.com/CC-GPR-350/a3>. Remember to:

1.  Make your repository **private**
2.  Add me as a collaborator
3.  Set your `UNITY_EMAIL`, `UNITY_PASSWORD`, and `UNITY_SERIAL` repository
    secrets


# Goal

Enhance the Shooting Game by implementing several Force Generator classes.


# Rules

1.  “1” key rotates player counter-clockwise
2.  “2” key rotates player clockwise
3.  “W” key changes to the next weapon
4.  “Enter” key fires
5.  Several ForceGenerators effect the Projectiles only


# Game Functionality Enhancements

1.  While holding the left mouse button an attractive force should be active at that location
2.  While holding the right mouse button a repulsive force should be active at that location
3.  One weapon (at least) should fire a projectile attached by some kind of spring to
    another moveable particle
4.  One weapon (at least) should fire a projectile attached to a fixed point by a spring


# Game Code Enhancements

1.  Make sure Particle2D contains an accumulate forces Vector2
2.  Make sure the Particle2D has an addForce function
3.  Enhance Integrator component as necessary
4.  Create a ForceGenerator component
5.  Create additional ForceGenerators as necessary for the various types of forces
6.  Make sure ForceGenerators have an updateForce function
7.  Continue to simulate Gravity and drag (damping) by setting the acceleration and
    damping constant of the Projectiles upon creation
8.  Only projectiles should be effected by ForceGenerators


# Grading

Grades will be based on the above criteria, which will be assessed
automatically using the automated tests provided with the project files.

