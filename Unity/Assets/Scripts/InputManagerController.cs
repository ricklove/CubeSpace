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
        var result = _swipeHelper.AddConstantInput(mPos, Input.GetMouseButton(0) || Input.touchCount > 0);

        if (result.isClick)
        {
            var crane = FindCrane();
            crane.DropBlock();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _swipeHelper.StartInput(mPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            result = _swipeHelper.EndInput(mPos);
        }

        if (result.isSwipe)
        {
            _swipeHelper.EndInput(mPos);

            var dir = result.swipeDirection;
            var planet = FindPlanet();

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x > 0) { planet.Rotate(RotationDirection.Left); }
                else { planet.Rotate(RotationDirection.Right); }
            }
            else
            {
                if (dir.y > 0) { planet.Rotate(RotationDirection.Down); }
                else { planet.Rotate(RotationDirection.Up); }
            }

        }
        else
        {
            // Drop Block on enter
            if (Input.GetButtonDown("Fire1") && !Input.GetMouseButtonDown(0))
            {
                var crane = FindCrane();

                crane.DropBlock();
            }

            // TODO: Add click with position inputs
        }
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
    public float minContinueDistance = 0.001f;
    public float maxClickDistance = 0.05f;

    public bool shouldShowDebug = true;

    private float _startTime;
    private Vector2 _startPosition;
    private float _minAngle;
    private float _maxAngle;

    private Vector2 _lastPosition;
    private bool _wasDown = false;
    private Vector2 _downStartPosition;


    public SwipeResult AddConstantInput(Vector2 position, bool isDown)
    {
        if (shouldShowDebug)
        {
            Debug.Log("SwipeHelper.AddConstantInput ------- ");
        }


        var downChange = DownChange.None;

        if (!_wasDown && isDown) { downChange = DownChange.WentDown; }
        else if (_wasDown && !isDown) { downChange = DownChange.WentUp; }
        _wasDown = isDown;

        if (downChange == DownChange.WentDown)
        {
            if (shouldShowDebug)
            {
                Debug.Log("SwipeHelper.AddConstantInput: Mouse Down");
            }

            StartInput(position);

            _downStartPosition = _startPosition;

            return SwipeResult.none;
        }
        else if (downChange == DownChange.WentUp)
        {
            if (shouldShowDebug)
            {
                Debug.Log("SwipeHelper.AddConstantInput: Mouse Up");
            }

            var diffStart = position - _downStartPosition;

            if (diffStart.sqrMagnitude < maxClickDistance * maxClickDistance)
            {
                if (shouldShowDebug)
                {
                    Debug.Log("SwipeHelper.AddConstantInput: Mouse Click");
                }

                return new SwipeResult() { isClick = true };
            }
            else
            {
                return EndInput(position);
            }
        }



        if (_startTime == 0)
        {
            StartInput(position);
        }

        // Timeout
        var timeDiff = Time.time - _startTime;

        if (timeDiff > maxSwipeTime)
        {
            if (shouldShowDebug)
            {
                Debug.Log("SwipeHelper.AddConstantInput: Timeout: timeDiff=" + timeDiff);
            }

            return EndInput(position);
        }

        // Motionless
        var diff = position - _lastPosition;
        _lastPosition = position;

        if (diff.sqrMagnitude < minContinueDistance * minContinueDistance)
        {
            if (shouldShowDebug)
            {
                Debug.Log("SwipeHelper.AddConstantInput: Motionless: diff=" + diff);
            }

            return EndInput(position);
        }

        // If continuing
        ContinueInput(position);
        var result = DetectSwipe(position);

        // If cannot be swipe, end
        if (!result.isSwipe && !result.couldBeSwipe)
        {
            return EndInput(position);
        }

        return result;
    }

    public void StartInput(Vector2 position)
    {
        _startTime = Time.time;
        _startPosition = position;
        _minAngle = 360;
        _maxAngle = 0;

        if (shouldShowDebug)
        {
            Debug.Log("SwipeHelper.Start: _startTime=" + _startTime + " _startPosition" + _startPosition);
        }
    }

    public void ContinueInput(Vector2 position)
    {
        if (_startTime == 0)
        {
            StartInput(position);
        }

        // Calculate Angle
        var diff = position - _startPosition;

        if (diff.sqrMagnitude > 0.001f)
        {
            var angle = Vector2.Angle(Vector2.right, diff);

            _minAngle = Mathf.Min(angle, _minAngle);
            _maxAngle = Mathf.Max(angle, _maxAngle);

            if (shouldShowDebug)
            {
                Debug.Log("SwipeHelper.Continue: _minAngle=" + _minAngle + " _maxAngle" + _maxAngle);
            }
        }
    }

    public SwipeResult EndInput(Vector2 position)
    {
        ContinueInput(position);

        var result = DetectSwipe(position);

        _startTime = 0;

        if (shouldShowDebug)
        {
            Debug.Log("SwipeHelper.End: result=" + result);
        }

        return result;
    }

    private SwipeResult DetectSwipe(Vector2 position)
    {
        var diff = position - _startPosition;
        var timeDiff = Time.time - _startTime;

        if ((360 + _maxAngle - _minAngle) % 360 < maxAngleTolerance)
        {
            if (diff.sqrMagnitude > minSwipeDistance * minSwipeDistance
                && timeDiff < maxSwipeTime)
            {
                return new SwipeResult() { isSwipe = true, swipeDirection = position - _startPosition };
            }
            else
            {
                return new SwipeResult() { isSwipe = false, couldBeSwipe = true };
            }
        }

        return SwipeResult.none;
    }
}

public struct SwipeResult
{
    public static SwipeResult none = new SwipeResult() { isSwipe = false, couldBeSwipe = false };

    public bool isClick;
    public bool isSwipe;
    public bool couldBeSwipe;
    public Vector2 swipeDirection;

    public override string ToString()
    {
        if (!isSwipe) { return "Not a Swipe"; }
        else { return "SwipeDirection=" + swipeDirection; }

    }
}

public enum DownChange
{
    None,
    WentUp,
    WentDown
}