using UnityEngine;
using System.Collections;

public class CraneController : MonoBehaviour
{
    public static CraneController Instance;

    private GameObject _blockProto;
    private GameObject _attachedBlock;
    private PlanetController _planet;

    private float? timeToCreateBlock;

    void Awake()
    {
        if (Instance != null) { throw new UnityException("Only One Crane can be created per scene"); }
        Instance = this;
    }

    void Start()
    {
        _blockProto = transform.Find("BlockProto").gameObject;
        _blockProto.SetActive(false);
    }

    private PlanetController FindPlanet()
    {
        if (_planet == null)
        {
            _planet = PlanetController.Instance;
        }

        return _planet;
    }

    void Update()
    {
        if (_attachedBlock == null)
        {
            if (timeToCreateBlock == null)
            {
                timeToCreateBlock = Time.time + 1f;
            }

            if (Time.time > timeToCreateBlock)
            {
                timeToCreateBlock = null;
                _attachedBlock = CreateBlock();
            }
        }

        // Align block to planet: Block is already centered
    }

    public void DropBlock()
    {
        _planet.AddBlockToPlanet(_attachedBlock);
        _attachedBlock = null;
    }

    private GameObject CreateBlock()
    {
        var planet = FindPlanet();
        //var size = planet.GetPlanetSize();
        var width = planet.width;
        var height = planet.height;
        var depth = planet.depth;

        //Debug.Log("PlanetSize: " + size);

        // Choose random side
        var side = Random.Range(0, 6);
        var a = width;
        var b = height;

        switch (side)
        {
            case 0: a = width; b = height; break;
            case 1: a = height; b = width; break;
            case 2: a = width; b = depth; break;
            case 3: a = depth; b = width; break;
            case 4: a = height; b = depth; break;
            case 5:
            default: a = depth; b = height; break;
        }

        var block = (GameObject)Instantiate(_blockProto);
        block.transform.parent = transform;
        block.transform.localScale = new Vector3(a, b, 1);
        block.transform.localPosition = new Vector3(0, 0, 0);
        block.SetActive(true);

        return block;
    }
}
