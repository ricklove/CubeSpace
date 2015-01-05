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

    private Text _text;

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
        _text = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();

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
            _text.text = "" + Mathf.FloorToInt(remainingTime / 60) + ":" + Mathf.FloorToInt(remainingTime % 60).ToString("D2");

            if (remainingTime < 10)
            {
                _text.color = Color.red;
            }
            else
            {
                _text.color = Color.white;
                //_text.color = Color.green;
            }
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
    }

    public void RemoveTime(float time)
    {
        _timeToRun -= time;
    }


}
