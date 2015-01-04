using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour
{

    public float timeToRotate = 0.25f;

    public static PlanetController Instance;

    private GameObject _rotation;
    private GameObject _rotationTarget;

    void Awake()
    {
        if (Instance != null) { throw new UnityException("Only One Planet can be created per scene"); }
        Instance = this;
    }

    void Start()
    {
        _rotation = transform.Find("Rotation").gameObject;
        _rotationTarget = transform.Find("RotationTarget").gameObject;
    }

    void Update()
    {
        if (_rotation.transform.localRotation != _rotationTarget.transform.localRotation)
        {
            _rotation.transform.localRotation =
                Quaternion.Slerp(_rotation.transform.localRotation, _rotationTarget.transform.localRotation, Time.deltaTime / timeToRotate);
        }
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
    }

}

public enum RotationDirection
{
    Up,
    Down,
    Left,
    Right
}
