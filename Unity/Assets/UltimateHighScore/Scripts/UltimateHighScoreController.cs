using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UltimateHighScoreController : MonoBehaviour
{
    public static UltimateHighScoreController Instance;

    public ScoreData[] scores;

    private Canvas _messageCanvas;
    private GameObject _messagePrefab;
    private GameObject _messages;
    private Vector2 _messageScreenSize;

    private ScoreData GetScoreData(string id)
    {
        foreach (var s in scores)
        {
            if (s.id == id)
            { return s; }
        }
        throw new System.Exception("Score id not found: id=" + id);
    }

    void Awake()
    {
        Instance = this;

        _messageCanvas = transform.FindChild("MessageCanvas").GetComponent<Canvas>();
        _messages = _messageCanvas.transform.FindChild("Messages").gameObject;
        _messagePrefab = _messageCanvas.transform.FindChild("MessagePrefab").gameObject;
        _messagePrefab.SetActive(false);

        var scalar = _messageCanvas.GetComponent<CanvasScaler>();
        _messageScreenSize = scalar.referenceResolution;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ResetScores()
    {

    }

    public void AddScore(string id, float scoreChange, Vector3 worldPosition)
    {
        var sData = GetScoreData(id);
        sData.score += scoreChange;

        // Show score in text
        sData.text.text = "" + sData.score;

        // Show Score message
        var mObj = (GameObject)Instantiate(_messagePrefab);
        var mRect = mObj.GetComponent<RectTransform>();
        var uiPos = ToUIPosition(worldPosition);

        var mText = mObj.GetComponent<Text>();
        mText.text = "+" + scoreChange;

        mRect.anchoredPosition = uiPos;
        mObj.transform.parent = _messages.transform;

        this.FadeOut(mText, 1.5f);
        this.FloatUp(mRect, _messageScreenSize.y * 0.2f, 2f);
        Destroy(mText, 5);

        mObj.SetActive(true);
    }



    private Vector2 ToUIPosition(Vector3 worldPosition)
    {
        var cam = Camera.mainCamera;
        var screenPos = cam.WorldToScreenPoint(worldPosition);
        var screenRatio = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        var uiPos = new Vector2(screenRatio.x * _messageScreenSize.x, screenRatio.y * _messageScreenSize.y);

        return uiPos;
    }


}

[System.Serializable]
public class ScoreData
{
    public string id;
    public float score;
    public Text text;
    public GameObject comboBar;
}

public class UHS
{
    public static UltimateHighScoreController Instance { get { return UltimateHighScoreController.Instance; } }
}

public static class EffectsHelper
{
    public static void FadeOut(this MonoBehaviour monoBehavior, Text message, float timeSpan)
    {
        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            var c = message.color;
            message.color = new Color(c.r, c.g, c.b, 1 - ratio);
        }));
    }


    public static void FloatUp(this MonoBehaviour monoBehavior, RectTransform transform, float distance, float timeSpan)
    {
        var startY = transform.anchoredPosition.y;

        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, startY + distance * ratio);
        }));
    }


    private static IEnumerator DoOverTime(float duration, TransitionAction doAction)
    {
        var startTime = Time.time;
        var ratioComplete = 0f;

        while (ratioComplete < 1)
        {
            doAction(ratioComplete);
            yield return null;

            ratioComplete = (Time.time - startTime) / duration;
        }

        doAction(1);
    }

    public delegate void TransitionAction(float ratioComplete);
}