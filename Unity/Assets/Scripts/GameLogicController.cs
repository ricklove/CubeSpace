using UnityEngine;
using System.Collections;

public class GameLogicController : MonoBehaviour
{
    private GameTimerController _gameTimer;
    private GameState _gameState;

    void Start()
    {
        _gameTimer = FindObjectOfType<GameTimerController>();
        _gameState = GameState.Startup;

        CraneController.Instance.BlockDropped += Crane_BlockDropped;
        CraneController.Instance.BlockDropFailed += Instance_BlockDropFailed;
    }

    void Instance_BlockDropFailed()
    {
        _gameTimer.RemoveTime(5);
    }

    void Crane_BlockDropped()
    {
        _gameTimer.AddTime(3);
    }

    void Update()
    {
        switch (_gameState)
        {
            case GameState.Startup:
                UpdateStartup();
                break;
            case GameState.Title:
                UpdateTitle();
                break;
            case GameState.Setup:
                UpdateSetup();
                break;
            case GameState.Play:
                UpdatePlay();
                break;
            case GameState.GameOver:
                UpdateGameOver();
                break;
            default:
                UpdateStartup();
                break;
        }

    }


    private void UpdateTitle()
    {
        _gameState = GameState.Setup;
    }

    private void UpdateStartup()
    {
        _gameState = GameState.Title;
    }

    private void UpdateSetup()
    {
        PlanetController.Instance.Reset();
        CraneController.Instance.Reset();
        _gameTimer.ResetTimer(60);
        _gameTimer.StartTimer();
        _gameState = GameState.Play;
    }

    private void UpdatePlay()
    {
        if (_gameTimer.RemainingTime < 0)
        {
            _gameState = GameState.GameOver;
        }
    }

    private void UpdateGameOver()
    {
        _gameState = GameState.Setup;
    }
}

public enum GameState
{
    Startup,
    Title,
    Setup,
    Play,
    GameOver

}
