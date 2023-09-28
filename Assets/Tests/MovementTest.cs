using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.TestTools.Utils;
using static TestHelpers;
using System.Collections.Generic;
using System.Linq;

public class MovementTest : InputTestFixture
{
    public struct IntegratorTestData
    {
        public Vector3 startPosition;
        public Vector2 startVelocity;
        public List<Vector2> constantForces;
        public float damping;
        public Vector2 gravity;
        public float inverseMass;
        public Vector3 expectedPosition;
        public Vector2 expectedVelocity;
        public float dt;
        public Dictionary<int, List<Vector2>> frameSpecificForces;

        public static IntegratorTestData Blank
        {
            get => new IntegratorTestData
            {
                startPosition = Vector3.zero,
                startVelocity = Vector2.zero,
                constantForces = new List<Vector2>(),
                damping = 1f,
                gravity = Vector2.zero,
                inverseMass = 1f,
                expectedPosition = Vector3.zero,
                expectedVelocity = Vector2.zero,
                dt = 1f,
                frameSpecificForces = new Dictionary<int, List<Vector2>>()
            };
        }

        public IntegratorTestData StartPosition(Vector3 position)
        {
            this.startPosition = position;
            return this;
        }

        public IntegratorTestData StartVelocity(Vector2 velocity)
        {
            this.startVelocity = velocity;
            return this;
        }

        public IntegratorTestData AddConstantForce(Vector2 force)
        {
            constantForces.Add(force);
            return this;
        }

        public IntegratorTestData ConstantForces(Vector2[] forces)
        {
            this.constantForces = new List<Vector2>(forces);
            return this;
        }

        public IntegratorTestData AddConstantForces(Vector2[] forces)
        {
            constantForces.AddRange(forces);
            return this;
        }

        public IntegratorTestData Damping(float damping)
        {
            this.damping = damping;
            return this;
        }

        public IntegratorTestData Gravity(Vector2 gravity)
        {
            this.gravity = gravity;
            return this;
        }

        public IntegratorTestData InverseMass(float invMass)
        {
            this.inverseMass = invMass;
            return this;
        }

        public IntegratorTestData Mass(float mass)
        {
            this.inverseMass = 1.0f / mass;
            return this;
        }

        public IntegratorTestData ExpectedPosition(Vector3 position)
        {
            this.expectedPosition = position;
            return this;
        }

        public IntegratorTestData ExpectedVelocity(Vector2 velocity)
        {
            this.expectedVelocity = velocity;
            return this;
        }

        public IntegratorTestData Dt(float dt)
        {
            this.dt = dt;
            return this;
        }

        public IntegratorTestData AddFrameSpecificForce(int frame, Vector2 force, int duration=1)
        {
            for (int i = 0; i < duration; i++)
            {
                List<Vector2> forcesOnFrame;
                if (!frameSpecificForces.TryGetValue(frame + i, out forcesOnFrame))
                {
                    forcesOnFrame = new List<Vector2>();
                }
                forcesOnFrame.Add(force);
                frameSpecificForces[frame + i] = forcesOnFrame;
            }

            return this;
        }

    }

    public static IntegratorTestData[] integratorTestDatas = new IntegratorTestData[]
    {
        IntegratorTestData.Blank
            .StartVelocity(Vector2.right * 3f)
            .ExpectedPosition(Vector2.right * 3f)
            .ExpectedVelocity(Vector2.right * 3f)
            .Dt(1),
        IntegratorTestData.Blank
            .AddConstantForce(Vector2.down * 9.8f)
            .Dt(5f)
            .ExpectedPosition(new Vector3(0, -122.00980f, 0))
            .ExpectedVelocity(new Vector2(0, -48.99986f)),
        IntegratorTestData.Blank
            .StartVelocity(Vector3.right * 5.0f)
            .Mass(20f)
            .AddFrameSpecificForce(2, Vector2.left * 400.0f, 10)
            .ExpectedPosition(new Vector3(2.60000f, 0.00000f, 0.00000f))
            .ExpectedVelocity(new Vector2(1.00000f, 0.00000f))
            .Dt(2f),
        IntegratorTestData.Blank
            .StartPosition(Vector2.right + Vector2.up * 1.3f)
            .Mass(0.5f)
            .AddConstantForce(Vector2.one * -1.33f)
            .ExpectedPosition(new Vector3(0.68080f, 0.98080f, 0.00000f))
            .ExpectedVelocity(new Vector2(-1.33000f, -1.33000f))
            .Dt(0.5f),
        IntegratorTestData.Blank
            .StartVelocity(Vector2.up * 4.0f)
            .Mass(5f)
            .Gravity(Vector2.down * 3f)
            .AddFrameSpecificForce(0, Vector2.right * 10f, 10)
            .AddFrameSpecificForce(0, Vector2.left * 10f, 10)
            .AddFrameSpecificForce(30, Vector2.left * 30f, 50)
            .ExpectedPosition(new Vector3(-0.45600f, 2.53000f, 0.00000f))
            .ExpectedVelocity(new Vector2(-2.40000f, 1.00000f))
            .Dt(1f),
        IntegratorTestData.Blank
            .StartPosition(Vector2.right * 300f)
            .Mass(0.001f)
            .Damping(0.7f)
            .Gravity(Vector2.left * 20f)
            .StartVelocity(Vector2.left * 5f)
            .AddConstantForce(Vector2.left + Vector2.up * 3f)
            .ExpectedPosition(new Vector3(-2101.42900f, 7038.60700f, 0.00000f))
            .ExpectedVelocity(new Vector2(-1683.39800f, 4945.13600f))
            .Dt(2.5f),
    };

    public static Particle2D SetUpParticle(IntegratorTestData testData)
    {
        Particle2D particle = new GameObject().AddComponent<Particle2D>();

        particle.transform.position = testData.startPosition;
        particle.velocity = testData.startVelocity;
        particle.damping = testData.damping;
        particle.gravity = testData.gravity;
        particle.inverseMass = testData.inverseMass;

        return particle;
    }

    public static void ApplyForces(IntegratorTestData testData, Particle2D particle, int frame)
    {
        testData.constantForces.ForEach(force => particle.AddForce(force));
        testData.frameSpecificForces
            .GetValueOrDefault(frame, new List<Vector2>())
            .ForEach(force => particle.AddForce(force));
    }

    public static void TestParticle(IntegratorTestData testData, Particle2D particle)
    {
        AssertVector3sEqual(particle.transform.position, testData.expectedPosition);
        AssertVector3sEqual(particle.velocity, testData.expectedVelocity);
        Object.Destroy(particle);
    }

    [Test]
    public void IntegratorTest([ValueSource("integratorTestDatas")] IntegratorTestData testData)
    {
        Particle2D particle = SetUpParticle(testData);

        int iterations = Mathf.RoundToInt(testData.dt / Time.fixedDeltaTime);

        for (int i = 0; i < iterations; i++)
        {
            ApplyForces(testData, particle, i);
            Integrator.Integrate(particle, Time.fixedDeltaTime);
            particle.ClearForces();
        }

        TestParticle(testData, particle);
    }

    public struct SpringForceTestData
    {
        public IntegratorTestData particleData;
        public IntegratorTestData otherParticleData;
        public float restLength;
        public float k;
        public bool shouldLinkSprings;

        public static SpringForceTestData Blank
        { get => new SpringForceTestData
        {
            particleData = IntegratorTestData.Blank,
            otherParticleData = IntegratorTestData.Blank,
            restLength = 1,
            k = 1,
            shouldLinkSprings = false,
        };
        }

        public SpringForceTestData Particle(IntegratorTestData testData)
        {
            particleData = testData;
            return this;
        }

        public SpringForceTestData OtherParticle(IntegratorTestData testData)
        {
            otherParticleData = testData;
            return this;
        }

        public SpringForceTestData RestLength(float rest)
        {
            restLength = rest;
            return this;
        }

        public SpringForceTestData K(float k)
        {
            this.k = k;
            return this;
        }

        public SpringForceTestData ShouldLinkSprings(bool y)
        {
            this.shouldLinkSprings = y;
            return this;
        }
    }

    public static SpringForceTestData[] springForceTestData = new SpringForceTestData[] {
        SpringForceTestData.Blank
            .Particle(IntegratorTestData.Blank
                            .Gravity(Vector2.down * 9.8f)
                            .ExpectedPosition(new Vector3(8.14561f, -16.46103f, 0.00000f))
                            .ExpectedVelocity(new Vector2(7.06956f, -13.01811f))
                            .Dt(2f))
            .OtherParticle(IntegratorTestData.Blank
                                 .StartPosition(Vector3.right * 10.0f))
            .RestLength(5f),
        SpringForceTestData.Blank
            .Particle(IntegratorTestData.Blank
                            .AddConstantForce(Vector2.one * 3f)
                            .ExpectedPosition(new Vector3(-3.77373f, 26.69310f, 0.00000f))
                            .ExpectedVelocity(new Vector2(-26.32135f, 22.53416f))
                            .Dt(5f))
            .OtherParticle(IntegratorTestData.Blank
                            .Gravity(Vector2.up)
                            .AddFrameSpecificForce(5, Vector2.one * -1.7f, 25)
                            .StartVelocity(Vector2.left * 5f))
            .ShouldLinkSprings(true)
            .K(5.5f)
            .RestLength(15f),
        SpringForceTestData.Blank
            .Particle(IntegratorTestData.Blank
                            .StartVelocity(Vector2.right * 200f)
                            .ExpectedPosition(new Vector3(192.83630f, 0.00000f, 0.00000f))
                            .ExpectedVelocity(new Vector2(-138.25960f, 0.00000f))
                            .Dt(3f))
            .OtherParticle(IntegratorTestData.Blank
                            .StartVelocity(Vector2.left * 200f))
            .ShouldLinkSprings(true)
            .K(0.3f)
            .RestLength(0.2f)
    };

    [Test]
    public void SpringForceTest([ValueSource("springForceTestData")] SpringForceTestData testData)
    {
        Particle2D particle = SetUpParticle(testData.particleData);
        Particle2D otherParticle = SetUpParticle(testData.otherParticleData);

        {
            SpringForce spring = particle.gameObject.AddComponent<SpringForce>();
            spring.springConstant = testData.k;
            spring.restLength = testData.restLength;
            spring.other = otherParticle.transform;
        }

        if (testData.shouldLinkSprings)
        {
            SpringForce spring = otherParticle.gameObject.AddComponent<SpringForce>();
            spring.springConstant = testData.k;
            spring.restLength = testData.restLength;
            spring.other = particle.transform;
        }

        int iterations = Mathf.RoundToInt(testData.particleData.dt / Time.fixedDeltaTime);
        for (int i = 0; i < iterations; i++)
        {
            ApplyForces(testData.particleData, particle, i);
            ApplyForces(testData.otherParticleData, otherParticle, i);
            particle.DoFixedUpdate(Time.fixedDeltaTime);
            otherParticle.DoFixedUpdate(Time.fixedDeltaTime);
        }

        // Only test first particle -- second particle is just to
        // make system more complex for first one.
        TestParticle(testData.particleData, particle);
    }

    public struct AttractorForceTestData
    {
        public IntegratorTestData particleData;
        public IntegratorTestData target;
        public float power;
        public string name;

        public override string ToString()
        {
            return name;
        }

        public static AttractorForceTestData Blank
        {
            get =>
                new AttractorForceTestData
                {
                    particleData = IntegratorTestData.Blank,
                };
        }

        public AttractorForceTestData Particle(IntegratorTestData particle)
        {
            this.particleData = particle;
            return this;
        }

        public AttractorForceTestData Target(IntegratorTestData particle)
        {
            this.target = particle;
            return this;
        }

        public AttractorForceTestData Power(float power)
        {
            this.power = power;
            return this;
        }

        public AttractorForceTestData Name(string name)
        {
            this.name = name;
            return this;
        }

    }

    public static AttractorForceTestData[] attractorForceTestData = new AttractorForceTestData[] {
        AttractorForceTestData.Blank
            .Particle(IntegratorTestData.Blank
                .Dt(3f)
                .ExpectedPosition(new Vector3(-8.86139f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.77936f, 0.00000f))
                .StartPosition(Vector2.left * 10f))
            .Target(IntegratorTestData.Blank
                .StartPosition(Vector2.right * 10f))
            .Power(100f)
            .Name("Attraction"),
        AttractorForceTestData.Blank
            .Particle(IntegratorTestData.Blank
                .Dt(3f)
                .ExpectedPosition(new Vector3(-11.09801f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(-0.72400f, 0.00000f))
                .StartPosition(Vector2.left * 10f))
            .Target(IntegratorTestData.Blank
                .StartPosition(Vector2.right * 10f))
            .Power(-100f)
            .Name("Repulsion"),
        AttractorForceTestData.Blank
            .Particle(IntegratorTestData.Blank
                .Dt(3f)
                .Damping(0.75f)
                .Gravity(Vector2.one * 1.5f)
                .ExpectedPosition(new Vector3(-5.80606f, 5.17637f, 0.00000f))
                .ExpectedVelocity(new Vector2(2.41742f, 3.08670f))
                .StartPosition(Vector2.left * 10f))
            .Target(IntegratorTestData.Blank
                .StartPosition(Vector2.right * 10f))
            .Power(-100f)
            .Name("Attraction with Damping, Gravity"),
    };

    [Test]
    public void TestAttractorForce([ValueSource("attractorForceTestData")] AttractorForceTestData testData)
    {
        Particle2D particle = SetUpParticle(testData.particleData);
        Particle2D target = SetUpParticle(testData.target);

        AttractorForce force = particle.gameObject.AddComponent<AttractorForce>();
        force.power = testData.power;

        int iterations = Mathf.RoundToInt(testData.particleData.dt / Time.fixedDeltaTime);

        for (int i = 0; i < iterations; i++)
        {
            force.targetPos = target.transform.position;
            ApplyForces(testData.particleData, particle, i);
            ApplyForces(testData.target, target, i);
            particle.DoFixedUpdate(Time.fixedDeltaTime);
            target.DoFixedUpdate(Time.fixedDeltaTime);
            particle.ClearForces();
            target.ClearForces();
        }

        TestParticle(testData.particleData, particle);
    }


    [UnityTest]
    public IEnumerator FixedUpdateTest([ValueSource("integratorTestDatas")] IntegratorTestData testData)
    {
        Particle2D particle = SetUpParticle(testData);

        int iterations = Mathf.RoundToInt(testData.dt / Time.fixedDeltaTime);

        for (int i = 0; i < iterations; i++)
        {
            ApplyForces(testData, particle, i);
            yield return new WaitForFixedUpdate();
            particle.ClearForces();
        }

        TestParticle(testData, particle);
    }
}
