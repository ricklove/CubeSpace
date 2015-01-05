using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTimerController : MonoBehaviour
{
    public delegate void TimerAction();
    public event TimerAction OnStarted;
    public event TimerAction OnFinished;

    private bool _isRunning;
    private float _timeAtStart;
    private float _timeToRun;

    private Text _timeRemaining;
    private Text _timeRemainingChanged;

    public float RemainingTime
    {
        get
        {
            var elapsedTime = Time.time - _timeAtStart;
            var remainingTime = _timeToRun - elapsedTime;
            return remainingTime;
        }
    }


    void Start()
    {
        _timeRemaining = GetComponentInChildren<Canvas>().transform.FindChild("Time").GetComponent<Text>();
        _timeRemainingChanged = GetComponentInChildren<Canvas>().transform.FindChild("Change").GetComponent<Text>();

        // TESTING
        ResetTimer(15);
        StartTimer();
    }

    void Update()
    {
        var remainingTime = RemainingTime;

        if (_isRunning)
        {
            if (remainingTime < 0)
            {
                remainingTime = 0;
                _isRunning = false;

                if (OnFinished != null)
                {
                    OnFinished();
                }
            }

            // Show remaining time
            _timeRemaining.text = "" + Mathf.FloorToInt(remainingTime / 60) + ":" + Mathf.FloorToInt(remainingTime % 60).ToString("D2");

            if (remainingTime < 10)
            {
                _timeRemaining.color = Color.red;
            }
            else
            {
                _timeRemaining.color = Color.white;
                //_text.color = Color.green;
            }

            // Fade out change
            //_timeRemainingChanged.color = _timeRemainingChanged.color - new Color(0, 0, 0, 0.05f);
            _timeRemainingChanged.color *= new Color(1, 1, 1, 0.95f);

        }
    }

    public void ResetTimer(float time)
    {
        _timeToRun = time;
        _isRunning = false;
    }

    public void StartTimer()
    {
        _timeAtStart = Time.time;
        _isRunning = true;

        if (OnStarted != null)
        {
            OnStarted();
        }
    }

    public void AddTime(float time)
    {
        _timeToRun += time;
        _timeRemainingChanged.text = "+" + time;
        _timeRemainingChanged.color = Color.green;
    }

    public void RemoveTime(float time)
    {
        _timeToRun -= time;
        _timeRemainingChanged.text = "-" + time;
        _timeRemainingChanged.color = Color.red;
    }


}
