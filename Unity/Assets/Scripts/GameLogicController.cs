using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameLogicController : MonoBehaviour
{
    private GameTimerController _gameTimer;
    private GameState _gameState;

    private Text _cubeCount;
    private Text _cubeLength;
    private Text _highestCubeCount;

    private int _blockCount = 1;
    private int _lastCubeLength = -1;

    void Awake()
    {
        var score = transform.FindChild("Score");
        _cubeCount = score.FindChild("CubeCount").GetComponent<Text>();
        _cubeLength = score.FindChild("CubeLength").GetComponent<Text>();
        _highestCubeCount = score.FindChild("HighestCubeCount").GetComponent<Text>();
    }

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
        _blockCount++;
        _gameTimer.AddTime(3);
        CraneController.Instance.nextBlockColor = GetNextColor();

        ShowCubeScore();
    }

    public int HighestCubeCount
    {
        get
        {
            if (!PlayerPrefs.HasKey("HighestCubeCount")) { return 0; }

            return PlayerPrefs.GetInt("HighestCubeCount");
        }
        set { PlayerPrefs.SetInt("HighestCubeCount", value); }
    }

    private void ShowCubeScore()
    {
        // Update cube size

        var cubeLength = Mathf.Min(PlanetController.Instance.width, PlanetController.Instance.height, PlanetController.Instance.depth);

        cubeLength = Mathf.Max(1, cubeLength);

        if (_lastCubeLength != cubeLength)
        {
            var cubeCount = cubeLength * cubeLength * cubeLength;
            _cubeLength.text = "" + cubeLength + "*" + cubeLength + "*" + cubeLength;
            _cubeCount.text = "" + cubeCount;

            _cubeLength.color = GetNextColor();
            _cubeCount.color = GetNextColor();

            HighestCubeCount = Mathf.Max(HighestCubeCount, cubeCount);
            _highestCubeCount.text = "High Score: " + HighestCubeCount;
        }
        else
        {
            //_cubeLength.color = Color.white;
            //_cubeCount.color = Color.white;
        }
    }

    private Color GetNextColor()
    {
        //switch (_blockCount % 3)
        //{
        //    case 0: return Color.red;
        //    case 1: return Color.green;
        //    case 2: return Color.blue;
        //   default: return Color.white; // Not possible
        //}

        var cubeLength = Mathf.Min(PlanetController.Instance.width, PlanetController.Instance.height, PlanetController.Instance.depth);

        switch ((cubeLength - 1) % 6)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.magenta;
            case 5: return Color.cyan;
            default: return Color.white; // Not possible
        }
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
        _blockCount = 1;

        PlanetController.Instance.Reset();
        CraneController.Instance.nextBlockColor = GetNextColor();

        CraneController.Instance.Reset();
        _gameTimer.ResetTimer(60);
        _gameTimer.StartTimer();
        _gameState = GameState.Play;

        ShowCubeScore();
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
