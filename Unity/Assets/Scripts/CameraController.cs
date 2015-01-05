using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private PlanetController _planet;
    private CraneController _crane;

    void Start()
    {

    }

    void Update()
    {

        var planet = FindPlanet();
        var crane = FindCrane();
        var cam = GetComponent<Camera>();

        var maxSize = Mathf.Max(planet.width, planet.height, planet.depth);

        cam.transform.position = new Vector3(0, 0, -5 - maxSize);

        //// Zoom to world size
        //var planetBottomLeft = new Vector3(-0.5f * planet.width, -0.5f * planet.height, 0);

        //var attachedBlock = crane._attachedBlock;

        //if (attachedBlock == null) { attachedBlock = crane.gameObject; }

        //var craneBlockSize = attachedBlock.transform.localScale;
        //var craneBlockPos = attachedBlock.transform.position;
        //var craneBottomLeft = new Vector3(-0.5f * craneBlockSize.x, -0.5f * craneBlockSize.y, craneBlockPos.z);

        //var viewPortAtCraneDepth = cam.ViewportToWorldPoint(new Vector3(-1, -1, -(cam.transform.position.z - craneBlockPos.z)));
        //var viewPortAtPlanetDepth = cam.ViewportToWorldPoint(new Vector3(-1, -1, -cam.transform.position.z));
        //Debug.Log("viewPortAtCraneDepth:" + viewPortAtCraneDepth);
        //Debug.Log("viewPortAtPlanetDepth:" + viewPortAtPlanetDepth);



        //var viewportPlanetBottomLeft = cam.WorldToViewportPoint(planetBottomLeft);
        //var viewportCraneBottomLeft = cam.WorldToViewportPoint(craneBottomLeft);

        ////Debug.Log("planetBottomLeft:" + planetBottomLeft);
        ////Debug.Log("viewportPlanetBottomLeft:" + viewportPlanetBottomLeft);
        ////Debug.Log("craneBottomLeft:" + craneBottomLeft);
        ////Debug.Log("viewportCraneBottomLeft:" + viewportCraneBottomLeft);

        //if (viewportPlanetBottomLeft.x < -1 || viewportPlanetBottomLeft.y < -1
        //    || viewportCraneBottomLeft.x < -1 || viewportCraneBottomLeft.y < -1)
        //{
        //    cam.transform.position += new Vector3(0, 0, -1f);

        //    viewportPlanetBottomLeft = cam.WorldToViewportPoint(planetBottomLeft);
        //    viewportCraneBottomLeft = cam.WorldToViewportPoint(craneBottomLeft);
        //}

    }

    private PlanetController FindPlanet()
    {
        if (_planet == null)
        {
            _planet = PlanetController.Instance;
        }

        return _planet;
    }

    private CraneController FindCrane()
    {
        if (_crane == null)
        {
            _crane = CraneController.Instance;
        }

        return _crane;
    }
}
