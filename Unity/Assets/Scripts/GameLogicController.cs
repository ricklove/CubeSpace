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
    private int _lastScore = -1;

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

    public int HighestScore
    {
        get
        {
            if (!PlayerPrefs.HasKey("HighestScore")) { return 0; }

            return PlayerPrefs.GetInt("HighestScore");
        }
        set { PlayerPrefs.SetInt("HighestScore", value); }
    }

    private void ShowCubeScore()
    {
        // Update cube size
        var w = PlanetController.Instance.width;
        var h = PlanetController.Instance.height;
        var d = PlanetController.Instance.depth;

        var cubeLength = Mathf.Min(w, h, d);
        cubeLength = Mathf.Max(1, cubeLength);
        var cubeCount = cubeLength * cubeLength * cubeLength;

        var blockCount = w * h * d;
        var score = blockCount;

        if (_lastScore != score)
        {
            var change = score - _lastScore;
            _lastScore = score;

            UHS.Instance.AddScore("Score", change, PlanetController.Instance.NextBlockCenter);

            _cubeCount.text = "" + blockCount;
            _cubeLength.text = "";

            //_cubeCount.text = "" + blockCount + " (" + w + "*" + h + "*" + d + ")";
            //_cubeLength.text = "Cube: " + cubeCount + " (" + cubeLength + "*" + cubeLength + "*" + cubeLength + ")";

            _cubeCount.color = GetNextColor();
            //            _cubeLength.color = GetNextColor();

            HighestScore = Mathf.Max(HighestScore, score);
            _highestCubeCount.text = "High Score: " + HighestScore;
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
