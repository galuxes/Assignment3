using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Transform pointDirection;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private List<GameObject> weapons = new List<GameObject>();

    int weaponIndex = 0;

    // TODO: Make sure to set these in the scene!
    [SerializeField]
    private GameObject SpringObject;

    [SerializeField]
    private GameObject ForceObject;

    /// <summary>
    /// The direction of the initial velocity of the fired projectile. That is,
    /// this is the direction the gun is aiming in.
    /// </summary>
    public Vector3 FireDirection
    {
        get
        {
            return pointDirection.up;
        }
    }

    /// <summary>
    /// The position in world space where a projectile will be spawned when
    /// Fire() is called.
    /// </summary>
    public Vector3 SpawnPosition
    {
        get
        {
            return pointDirection.position;
        }
    }

    public GameObject CurrentWeapon
    {
        get
        {
            return weapons[weaponIndex];
        }
    }

    /// <summary>
    /// Spawns the currently active projectile, firing it in the direction of
    /// FireDirection.
    /// </summary>
    /// <returns>The newly created GameObject.</returns>
    public GameObject Fire()
    {
        return Fire(CurrentWeapon);
    }

    public GameObject Fire(GameObject prototype)
    {
        GameObject firedObject = Instantiate(prototype, SpawnPosition, Quaternion.identity);
        firedObject.GetComponent<Particle2D>().velocity = FireDirection * 10f;
        return firedObject;
    }

    /// <summary>
    /// Moves to the next weapon. If the last weapon is selected, calling this
    /// again will roll over to the first weapon again. For example, if there
    /// are 4 weapons, calling this 4 times will end up with the same weapon
    /// selected as if it was called 0 times.
    /// </summary>
    public void CycleNextWeapon()
    {
        weaponIndex = (weaponIndex + 1) % weapons.Count;
    }

    /// <summary>
    /// Spawns a particle that has a SpringForce component attached. Spawns
    /// another particle, and attaches the SpringForce's "other" variable
    /// to the other particle's transform.
    /// </summary>
    /// <returns>The created spring object (NOT the spring object's target)</returns>
    public GameObject FirePairedSpringWeapon()
    {
        // TODO: YOUR CODE HERE
        return null;
    }


    /// <summary>
    /// Spawns a particle that has a SpringForce component attached. The spring
    /// force component should have its "other" variable set to some transform
    /// in the scene (Hint: You can use the gun object's transform for this!)
    /// </summary>
    /// <returns>The created spring object</returns>
    public GameObject FireStaticSpringWeapon()
    {
        // TODO: YOUR CODE HERE
        return null;
    }

    /// <summary>
    /// Spawns a particle with an AttractorForce and a ForceMouseController
    /// component attached. The force object should be attracted to the mouse
    /// when the left mouse button is held down.
    /// </summary>
    /// <returns>The fired object.</returns>
    public GameObject FireAttractorForceWeapon()
    {
        // TODO: YOUR CODE HERE

        return null;
    }

    /// <summary>
    /// Spawns a particle with an AttractorForce and a ForceMouseController
    /// component attached. The force object should be repelled by the mouse
    /// when the right mouse button is held down.
    /// </summary>
    /// <returns></returns>
    public GameObject FireRepulsiveForceWeapon()
    {
        // TODO: YOUR CODE HERE

        return null;
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.IsPressed())
        {
            transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
        }

        if (Keyboard.current.digit2Key.IsPressed())
        {
            transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.back);
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Fire();
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            CycleNextWeapon();
        }
    }
}
