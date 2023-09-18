using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneTests : InputTestFixture
{
    Mouse mouse { get => Mouse.current; }
    const string testSceneId = "Assets/Scenes/TestScene.unity";

    public override void Setup()
    {
        base.Setup();
        InputSystem.AddDevice<Keyboard>();
        InputSystem.AddDevice<Mouse>();
        SceneManager.LoadScene(testSceneId);
    }

    // Test parser expects all tests to be iterated, so we add this
    // variable to make each test run once.
    public static int[] dummyData = new int[] { 0 };


    [UnityTest]
    public IEnumerator AttractorTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        int preFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        int preFireForce = Object.FindObjectsOfType<AttractorForce>().Length;
        gun.FireAttractorForceWeapon();
        Press(Mouse.current.leftButton);
        yield return null;
        int postFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        Assert.That(postFireCount, Is.GreaterThan(preFireCount), "No additional Particle2D's created after 'enter' key pressed.");
        Assert.That(Object.FindObjectsOfType<Rigidbody>().Length, Is.EqualTo(0), "At least one Rigidbody exists in the scene.");
        Assert.That(Object.FindObjectsOfType<Rigidbody2D>().Length, Is.EqualTo(0), "Rigidbody2D exists in the scene.");
        Particle2D particle = Object.FindObjectsOfType<Particle2D>()[0];

        AttractorForce[] forces = Object.FindObjectsOfType<AttractorForce>();
        Assert.That(forces.Length, Is.GreaterThan(preFireForce), "No additional AttractorForce's created after FireAttractorForceWeapon() called.");
        AttractorForce force = forces[0];
        Assert.That(force.enabled, Is.True, "Mouse is pressed but attractor force is not enabled.");
        Assert.That(force.power, Is.GreaterThan(0f), "Attractor force is repelling instead of attracting.");
        Release(Mouse.current.leftButton);
        yield return null;

        Assert.That(force.enabled, Is.False, "Mouse is released but attractor force is enabled.");
    }


    [UnityTest]
    public IEnumerator RepulsionTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        int preFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        int preFireForce = Object.FindObjectsOfType<AttractorForce>().Length;
        gun.FireRepulsiveForceWeapon();
        Press(Mouse.current.rightButton);
        yield return null;
        int postFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        Assert.That(postFireCount, Is.GreaterThan(preFireCount), "No additional Particle2D's created after 'enter' key pressed.");
        Assert.That(Object.FindObjectsOfType<Rigidbody>().Length, Is.EqualTo(0), "At least one Rigidbody exists in the scene.");
        Assert.That(Object.FindObjectsOfType<Rigidbody2D>().Length, Is.EqualTo(0), "Rigidbody2D exists in the scene.");
        Particle2D particle = Object.FindObjectsOfType<Particle2D>()[0];

        AttractorForce[] forces = Object.FindObjectsOfType<AttractorForce>();
        Assert.That(forces.Length, Is.GreaterThan(preFireForce), "No additional AttractorForce's created after FireAttractorForceWeapon() called.");
        AttractorForce force = forces[0];
        Assert.That(force.enabled, Is.True, "Mouse is pressed but attractor force is not enabled.");
        Assert.That(force.power, Is.LessThan(0f), "Repulsive force is attracting instead of repelling.");
        Release(Mouse.current.rightButton);
        yield return null;

        Assert.That(force.enabled, Is.False, "Mouse is released but attractor force is enabled.");
    }


    [UnityTest]
    public IEnumerator FixedSpringTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        int preFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        int preFireForce = Object.FindObjectsOfType<SpringForce>().Length;
        gun.FireStaticSpringWeapon();

        yield return null;
        int postFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        Assert.That(postFireCount, Is.GreaterThan(preFireCount), "No additional Particle2D's created after 'enter' key pressed.");
        Assert.That(Object.FindObjectsOfType<Rigidbody>().Length, Is.EqualTo(0), "At least one Rigidbody exists in the scene.");
        Assert.That(Object.FindObjectsOfType<Rigidbody2D>().Length, Is.EqualTo(0), "Rigidbody2D exists in the scene.");

        SpringForce[] forces = Object.FindObjectsOfType<SpringForce>();
        Assert.That(forces.Length, Is.GreaterThan(preFireForce), "No additional SpringForces's created after FireStaticSpringWeapon() called.");
        SpringForce force = forces[0];
        Assert.That(force.other, Is.Not.Null, "Spring has no attachment point.");
        Assert.That(force.other.GetComponent<Particle2D>(), Is.Null, "Static spring is attached to a particle's transform.");
    }

    [UnityTest]
    public IEnumerator PairedSpringTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        int preFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        int preFireForce = Object.FindObjectsOfType<SpringForce>().Length;
        gun.FirePairedSpringWeapon();

        yield return null;
        int postFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        Assert.That(postFireCount, Is.GreaterThan(preFireCount), "No additional Particle2D's created after 'enter' key pressed.");
        Assert.That(Object.FindObjectsOfType<Rigidbody>().Length, Is.EqualTo(0), "At least one Rigidbody exists in the scene.");
        Assert.That(Object.FindObjectsOfType<Rigidbody2D>().Length, Is.EqualTo(0), "Rigidbody2D exists in the scene.");

        SpringForce[] forces = Object.FindObjectsOfType<SpringForce>();
        Assert.That(forces.Length, Is.GreaterThan(preFireForce), "No additional SpringForces's created after FirePairedSpringWeapon() called.");
        SpringForce force = forces[0];
        Assert.That(force.other, Is.Not.Null, "Spring has no attachment point.");
        Assert.That(force.other.GetComponent<Particle2D>(), Is.Not.Null, "Static spring is not attached to another particle's transform.");
    }
}
