using UnityEngine;
using System.Collections;

public class InputManagerController : MonoBehaviour
{
    public float rotationsPerSecond = 1.0f;

    private PlanetController _planet;
    private CraneController _crane;
    private float _timeAtLastRotation;

    private SwipeHelper _swipeHelper;


    void Start()
    {
        _swipeHelper = new SwipeHelper();
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

    void Update()
    {
        var timePerRotation = 1.0f / rotationsPerSecond;

        // Button, Gamepad directions
        if (Input.GetAxis("Vertical") != 0
            || Input.GetAxis("Horizontal") != 0)
        {
            if (_timeAtLastRotation + timePerRotation < Time.time)
            {
                _timeAtLastRotation = Time.time;

                var planet = FindPlanet();

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

        // Detect swipe
        var mPos = SwipeHelper.ToScreenRatio(Input.mousePosition, new Vector2(Screen.width, Screen.height));
        var result = _swipeHelper.AddConstantInput(mPos);

        if (Input.GetMouseButtonDown(0))
        {
            _swipeHelper.StartInput(mPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            result = _swipeHelper.EndInput(mPos);
        }

        Debug.Log("mPos:" + mPos);
        Debug.Log("result:" + result);

        // Drop Block on enter
        if (Input.GetButtonDown("Fire1") && !Input.GetMouseButtonDown(0))
        {
            var crane = FindCrane();

            crane.DropBlock();
        }

        // TODO: Add click with position inputs

    }



}


public class SwipeHelper
{
    public static Vector2 ToScreenRatio(Vector2 position, Vector2 screenSize)
    {
        return new Vector2(position.x / screenSize.x, position.y / screenSize.y);
    }

    public float minSwipeDistance = 0.25f;
    public float maxSwipeTime = 2f;
    public float maxAngleTolerance = 30;
    public float minContinueDistance = 0.01f;

    private float _startTime;
    private Vector2 _startPosition;
    private float _minAngle;
    private float _maxAngle;

    private Vector2 _lastPosition;

    public SwipeResult AddConstantInput(Vector2 position)
    {
        if (_startTime == 0)
        {
            StartInput(position);
        }

        // Timeout
        var timeDiff = Time.time - _startTime;

        if (timeDiff > maxSwipeTime)
        {
            return EndInput(position);
        }

        // Motionless
        var diff = position - _lastPosition;

        if (diff.sqrMagnitude < minContinueDistance * minContinueDistance)
        {
            return EndInput(position);
        }

        // If continuing
        ContinueInput(position);
        return DetectSwipe(position);
    }

    public void StartInput(Vector2 position)
    {
        _startTime = Time.time;
        _startPosition = position;
    }

    public void ContinueInput(Vector2 position)
    {
        if (_startTime == 0)
        {
            StartInput(position);
        }

        // Calculate Angle
        var diff = position - _startPosition;
        var angle = Vector2.Angle(Vector2.right, diff);

        _minAngle = Mathf.Min(angle, _minAngle);
        _maxAngle = Mathf.Max(angle, _maxAngle);
    }

    public SwipeResult EndInput(Vector2 position)
    {
        ContinueInput(position);

        var result = DetectSwipe(position);

        _startTime = 0;

        return result;
    }

    private SwipeResult DetectSwipe(Vector2 position)
    {
        var diff = position - _startPosition;
        var timeDiff = Time.time - _startTime;

        if (diff.sqrMagnitude > minSwipeDistance * minSwipeDistance
            && timeDiff < maxSwipeTime)
        {
            if ((360 + _maxAngle - _minAngle) % 360 < maxAngleTolerance)
            {
                return new SwipeResult() { isSwipe = true, swipeDirection = position - _startPosition };
            }
            else
            {
                return SwipeResult.none;
            }
        }

        return SwipeResult.none;
    }
}

public struct SwipeResult
{
    public static SwipeResult none = new SwipeResult() { isSwipe = false };

    public bool isSwipe;
    public Vector2 swipeDirection;

    public override string ToString()
    {
        if (!isSwipe) { return "Not a Swipe"; }
        else { return "SwipeDirection=" + swipeDirection; }

    }
}