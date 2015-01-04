using UnityEngine;
using System.Collections;

public class InputManagerController : MonoBehaviour
{
    public float rotationsPerSecond = 1.0f;

    private PlanetController _planet;
    private float _timeAtLastRotation;


    void Start()
    {
    }

    private PlanetController FindPlanet()
    {
        if (_planet == null)
        {
            _planet = PlanetController.Instance;
            //_planet = transform.root.Find("Planet").GetComponent<PlanetController>();
        }

        return _planet;
    }

    void Update()
    {
        var planet = FindPlanet();

        var timePerRotation = 1.0f / rotationsPerSecond;

        if (Input.GetAxis("Vertical") != 0
            || Input.GetAxis("Horizontal") != 0)
        {
            if (_timeAtLastRotation + timePerRotation < Time.time)
            {
                _timeAtLastRotation = Time.time;

                // Rotate Planet
                if (Input.GetAxis("Vertical") > 0)
                {
                    planet.Rotate(RotationDirection.Up);
                }

                if (Input.GetAxis("Vertical") < 0)
                {
                    planet.Rotate(RotationDirection.Down);
                }

                if (Input.GetAxis("Horizontal") > 0)
                {
                    planet.Rotate(RotationDirection.Right);
                }

                if (Input.GetAxis("Horizontal") < 0)
                {
                    planet.Rotate(RotationDirection.Left);
                }
            }
        }

        // TODO: Drop Block
        if (Input.GetButtonDown("Fire1"))
        {

        }
    }


}
