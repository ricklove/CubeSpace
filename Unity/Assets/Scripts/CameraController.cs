using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {

        var planet = FindPlanet();

        // Move camera back
	}

    private PlanetController FindPlanet()
    {
        if (_planet == null)
        {
            _planet = PlanetController.Instance;
        }

        return _planet;
    }
}
