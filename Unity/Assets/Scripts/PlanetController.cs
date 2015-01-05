using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlanetController : MonoBehaviour
{
    public static PlanetController Instance;

    public float timeToRotate = 0.25f;

    public Axis WidthAxis;
    public Axis HeightAxis;
    public Axis DepthAxis;

    public float Width;
    public float Height;
    public float Depth;

    private GameObject _rotation;
    private GameObject _rotationTarget;
    private GameObject _xAxis;
    private GameObject _yAxis;
    private GameObject _zAxis;

    private GameObject _blocks;

    void Awake()
    {
        if (Instance != null) { throw new UnityException("Only One Planet can be created per scene"); }
        Instance = this;
    }

    void Start()
    {
        _rotation = transform.Find("Rotation").gameObject;
        _rotationTarget = transform.Find("RotationTarget").gameObject;
        _blocks = _rotation.transform.Find("Blocks").gameObject;

        _xAxis = _rotationTarget.transform.Find("X").gameObject;
        _yAxis = _rotationTarget.transform.Find("Y").gameObject;
        _zAxis = _rotationTarget.transform.Find("Z").gameObject;

    }

    void Update()
    {
        // Rotate to target
        if (_rotation.transform.localRotation != _rotationTarget.transform.localRotation)
        {
            _rotation.transform.localRotation =
                Quaternion.Slerp(_rotation.transform.localRotation, _rotationTarget.transform.localRotation, Time.deltaTime / timeToRotate);

            // Center position
            //var diff = _rotationTarget.transform.localPosition - _rotation.transform.localPosition;
            //var distance = diff.magnitude;
            //distance = Mathf.Min(distance, 0.5f * Time.deltaTime / timeToRotate);
            //_rotation.transform.localPosition = diff.normalized * distance;
            
            //_rotation.transform.localPosition = _rotationTarget.transform.localPosition;
        }


        // Update axis measures
        UpdateAxisMeasures();

    }

    private void UpdateAxisMeasures()
    {
        // Update axis measures
        var xVal = _xAxis.transform.position - _rotationTarget.transform.position;
        var yVal = _yAxis.transform.position - _rotationTarget.transform.position;
        var zVal = _zAxis.transform.position - _rotationTarget.transform.position;

        WidthAxis = GetAxis(xVal, yVal, zVal, (v) => { return v.x; });
        HeightAxis = GetAxis(xVal, yVal, zVal, (v) => { return v.y; });
        DepthAxis = GetAxis(xVal, yVal, zVal, (v) => { return v.z; });

        // Update Width, Height, Depth values
        var size = GetPlanetSize();

        Width = GetAxisLength(size, WidthAxis);
        Height = GetAxisLength(size, HeightAxis);
        Depth = GetAxisLength(size, DepthAxis);
    }

    private float GetAxisLength(Vector3 size, Axis axis)
    {
        if (axis == Axis.X) { return size.x; }
        else if (axis == Axis.Y) { return size.y; }
        else if (axis == Axis.Z) { return size.z; }

        throw new System.ArgumentException("No Axis");
    }

    private static Axis GetAxis(Vector3 x, Vector3 y, Vector3 z, System.Func<Vector3, float> doGetVal)
    {
        if (Mathf.Abs(doGetVal(x)) > 0.1f) { return Axis.X; }
        else if (Mathf.Abs(doGetVal(y)) > 0.1f) { return Axis.Y; }
        else if (Mathf.Abs(doGetVal(z)) > 0.1f) { return Axis.Z; }

        throw new System.ArgumentException("No Axis");
    }


    public void Rotate(RotationDirection direction)
    {
        Debug.Log("Rotate " + direction);

        var rObj = _rotationTarget;

        if (direction == RotationDirection.Up)
        {
            rObj.transform.RotateAround(rObj.transform.position, Vector3.right, -90);
        }
        else if (direction == RotationDirection.Down)
        {
            rObj.transform.RotateAround(rObj.transform.position, Vector3.right, 90);
        }
        else if (direction == RotationDirection.Left)
        {
            rObj.transform.RotateAround(rObj.transform.position, Vector3.up, -90);
        }
        else //if (direction == RotationDirection.Right)
        {
            rObj.transform.RotateAround(rObj.transform.position, Vector3.up, 90);
        }

        UpdateAxisMeasures();

        // TODO: Calculate the center from the core piece
        //_rotationTarget.transform.localPosition = new Vector3(
        //        Width * 0.5f,
        //        Height * 0.5f,
        //        Depth * 0.5f
        //        );
    }

    public Vector3 GetPlanetSize()
    {
        // Get block extents in each direction (block positions)
        var bBounds = new List<Bounds>();

        foreach (Transform b in _blocks.transform)
        {
            // TODO: FIX THIS
            var bounds = new Bounds(b.localPosition, new Vector3(1, 1, 1));
            //var bounds = new Bounds(b.localPosition, Vector3.zero);
            //var bounds = new Bounds(b.localPosition, b.localScale);
            //var bounds = b.GetComponent<BlockController>().GetWorldBounds();
            bBounds.Add(bounds);
            Debug.Log("Block Bounds:" + bounds + " min: " + bounds.min + "max: " + bounds.max);
        }

        var minX = bBounds.Min(b => b.min.x);
        var minY = bBounds.Min(b => b.min.y);
        var minZ = bBounds.Min(b => b.min.z);
        var maxX = bBounds.Max(b => b.max.x);
        var maxY = bBounds.Max(b => b.max.y);
        var maxZ = bBounds.Max(b => b.max.z);

        //return new Vector3(maxX - minX + 1, maxY - minY + 1, maxZ - minZ + 1);
        return new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
    }

    public void AddBlockToPlanet(GameObject block)
    {
        block.transform.parent = _blocks.transform;
    }

}

public enum RotationDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum Axis
{
    X,
    Y,
    Z
}