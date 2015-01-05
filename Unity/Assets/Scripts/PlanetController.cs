using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlanetController : MonoBehaviour
{
    public static PlanetController Instance;

    public float timeToRotate = 0.25f;
    public float timeToPosition = 0.5f;

    public Axis widthAxis;
    public Axis heightAxis;
    public Axis depthAxis;

    public float width;
    public float height;
    public float depth;

    public float widthCenter;
    public float heightCenter;
    public float depthCenter;

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
        }

        // Center position
        var diff = _rotationTarget.transform.localPosition - _rotation.transform.localPosition;

        if (diff.sqrMagnitude > 0)
        {
            var distance = diff.magnitude;
            var maxDistance = Time.deltaTime / timeToPosition;

            if (distance > maxDistance)
            {
                distance = maxDistance;
            }

            _rotation.transform.localPosition += diff.normalized * distance;
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

        widthAxis = GetAxis(xVal, yVal, zVal, (v) => { return v.x; });
        heightAxis = GetAxis(xVal, yVal, zVal, (v) => { return v.y; });
        depthAxis = GetAxis(xVal, yVal, zVal, (v) => { return v.z; });

        // Update Width, Height, Depth values
        var bounds = GetPlanetBounds();
        var size = bounds.size;
        var center = bounds.center;

        width = Mathf.Abs(GetAxisValue(size, widthAxis));
        height = Mathf.Abs(GetAxisValue(size, heightAxis));
        depth = Mathf.Abs(GetAxisValue(size, depthAxis));

        widthCenter = GetAxisValue(center, widthAxis);
        heightCenter = GetAxisValue(center, heightAxis);
        depthCenter = GetAxisValue(center, depthAxis);
    }

    private float GetAxisValue(Vector3 value, Axis axis)
    {
        if (axis == Axis.X) { return value.x; }
        else if (axis == Axis.XNegative) { return -value.x; }
        else if (axis == Axis.Y) { return value.y; }
        else if (axis == Axis.YNegative) { return -value.y; }
        else if (axis == Axis.Z) { return value.z; }
        else if (axis == Axis.ZNegative) { return -value.z; }

        throw new System.ArgumentException("No Axis");
    }

    private static Axis GetAxis(Vector3 x, Vector3 y, Vector3 z, System.Func<Vector3, float> doGetVal)
    {
        if (doGetVal(x) > 0.1f) { return Axis.X; }
        else if (doGetVal(x) < -0.1f) { return Axis.XNegative; }
        else if (doGetVal(y) > 0.1f) { return Axis.Y; }
        else if (doGetVal(y) < -0.1f) { return Axis.YNegative; }
        else if (doGetVal(z) > 0.1f) { return Axis.Z; }
        else if (doGetVal(z) < -0.1f) { return Axis.ZNegative; }

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

        // Calculate the center
        _rotationTarget.transform.localPosition = new Vector3(
                -widthCenter,
                -heightCenter,
                -depthCenter
                );
    }

    public Bounds GetPlanetBounds()
    {
        // Get block extents in each direction (block positions)
        var bBounds = new List<Bounds>();

        foreach (Transform b in _blocks.transform)
        {
            var bounds = new Bounds(b.localPosition, new Vector3(1, 1, 1));
            bBounds.Add(bounds);
            Debug.Log("Block Bounds:" + bounds + " min: " + bounds.min + "max: " + bounds.max);
        }

        var minX = bBounds.Min(b => b.min.x);
        var minY = bBounds.Min(b => b.min.y);
        var minZ = bBounds.Min(b => b.min.z);
        var maxX = bBounds.Max(b => b.max.x);
        var maxY = bBounds.Max(b => b.max.y);
        var maxZ = bBounds.Max(b => b.max.z);

        return new Bounds(
            new Vector3((maxX + minX) * 0.5f, (maxY + minY) * 0.5f, (maxZ + minZ) * 0.5f),
            new Vector3(maxX - minX, maxY - minY, maxZ - minZ));
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
    Z,
    XNegative,
    YNegative,
    ZNegative
}