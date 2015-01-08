using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UltimateHighScoreController : MonoBehaviour
{
    public static UltimateHighScoreController Instance;

    public ScoreData[] scores;

    private Canvas _messageCanvas;
    private GameObject _messageProto;
    private GameObject _messages;
    private Vector2 _messageScreenSize;

    private GameObject _coins;


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
        _messageProto = _messageCanvas.transform.FindChild("MessageProto").gameObject;
        _messageProto.SetActive(false);

        _coins = transform.FindChild("Coins").gameObject;

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

    internal void ResetScore(string id, float startScore)
    {
        var sData = GetScoreData(id);
        sData.score = startScore;
        sData.text.text = "" + sData.score;
    }

    public void AddScore(string id, float scoreChange, Vector3 worldPosition)
    {
        var sData = GetScoreData(id);
        var oldScore = sData.score;

        sData.score += scoreChange;

        // Show Score message
        var mObj = (GameObject)Instantiate(_messageProto);

        var mRect = mObj.GetComponent<RectTransform>();
        var uiPos = WorldToUIPosition(worldPosition);

        var mText = mObj.GetComponent<Text>();
        mText.text = "+" + scoreChange;

        mRect.anchoredPosition = uiPos;
        mObj.transform.parent = _messages.transform;

        this.FadeOut(mText, 1.5f);
        this.FloatUp(mRect, _messageScreenSize.y * 0.2f, 2f);
        Destroy(mText, 5);

        mObj.SetActive(true);

        // Show coins
        Debug.Log("sData.coinPrefab=" + sData.coinPrefab);

        if (sData.coinPrefab != null)
        {
            var fromPos = worldPosition;
            var toPos = RectToWorldPosition(sData);

            //var diff = toPos - fromPos;
            //var toPastPos = fromPos + toPos * 2;
            //toPos = toPastPos;


            var coinCount = scoreChange;
            var timeToShowCoins = 1.0f;
            var maxTimePerCoin = 0.2f;
            var timeToMove = 0.25f;
            var maxCoins = 25;

            coinCount = Mathf.Min(maxCoins, coinCount);

            //var timePerCoin = timeToShowCoins / coinCount;
            var timePerCoin = timeToShowCoins / coinCount;
            timePerCoin = Mathf.Min(maxTimePerCoin, timePerCoin);

            var scorePerCoin = scoreChange / coinCount;

            for (int i = 0; i < coinCount; i++)
            {
                var scoreTextAfterChange = "" + Mathf.CeilToInt(oldScore + (1 + i) * scorePerCoin);

                if (i == coinCount - 1)
                {
                    scoreTextAfterChange = "" + sData.score;
                }

                StartCoroutine(CreateCoin(sData.coinPrefab, sData.text, scoreTextAfterChange,
                    fromPos, toPos, timeToMove, i * timePerCoin));
            }

            // Show score in text
        }
        else
        {
            // Show score in text
            sData.text.text = "" + sData.score;
        }


    }



    private IEnumerator CreateCoin(GameObject coinPrefab, Text score, string scoreTextAfterChange, Vector3 startPos, Vector3 endPos, float timeToMove, float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("CreateCoin");


        var obj = (GameObject)Instantiate(coinPrefab);
        var transform = obj.transform;
        transform.parent = _coins.transform;
        transform.position = startPos;

        this.MoveThenHide(transform, endPos - startPos, timeToMove);
        obj.SetActive(true);

        yield return new WaitForSeconds(timeToMove);
        obj.SetActive(false);
        Destroy(obj, timeToMove + 0.1f);

        score.text = scoreTextAfterChange;
    }



    private Vector2 WorldToUIPosition(Vector3 worldPosition)
    {
        var cam = Camera.mainCamera;

        var screenPos = cam.WorldToScreenPoint(worldPosition);
        var screenRatio = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        var uiPos = new Vector2(screenRatio.x * _messageScreenSize.x, screenRatio.y * _messageScreenSize.y);

        return uiPos;
    }

    //private Vector2 UIToWorldPosition(Vector3 uiPosition)
    //{
    //    var cam = Camera.mainCamera;

    //    var screenRatio = new Vector2(uiPosition.x / _messageScreenSize.x, uiPosition.y / _messageScreenSize.y);
    //    var screenPos = new Vector2(screenRatio.x * Screen.width, screenRatio.y * Screen.height);
    //    var worldPos = cam.ScreenToWorldPoint(screenPos);

    //    return worldPos;
    //}

    private Vector3 RectToWorldPosition(ScoreData sData)
    {
        var cam = Camera.mainCamera;

        var screenRect = GetScreenRect(sData.text.GetComponent<RectTransform>());
        var screenPos = screenRect.center;

        // TODO: find out where center of rect is in world coordinates
        // Assume rect is at center of world
        var toPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, (cam.transform.position - Vector3.zero).magnitude - 1));
        //var toPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 3f));


        Debug.Log("sData.text.transform.position=" + sData.text.transform.position);
        Debug.Log("screenPos=" + screenPos);
        Debug.Log("toPos=" + toPos);

        return toPos;
    }


    // From: http://answers.unity3d.com/questions/781643/unity-46-beta-rect-transform-position-new-ui-syste.html
    private Vector3[] _tempCorners = new Vector3[4];
    public Rect GetScreenRect(RectTransform rectTransform)
    {
        var corners = _tempCorners;
        rectTransform.GetWorldCorners(corners);

        float xMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity;
        float yMin = float.PositiveInfinity;
        float yMax = float.NegativeInfinity;

        for (int i = 0; i < 4; i++)
        {
            // For Canvas mode Screen Space - Overlay there is no Camera; best solution I've found
            // is to use RectTransformUtility.WorldToScreenPoint() with a null camera.

            Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(rectTransform.parent.GetComponent<Canvas>().worldCamera, corners[i]);

            if (screenCoord.x < xMin)
                xMin = screenCoord.x;
            if (screenCoord.x > xMax)
                xMax = screenCoord.x;
            if (screenCoord.y < yMin)
                yMin = screenCoord.y;
            if (screenCoord.y > yMax)
                yMax = screenCoord.y;
        }

        Rect result = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

        return result;
    }






}

[System.Serializable]
public class ScoreData
{
    public string id;
    public float score;
    public Text text;
    public GameObject comboBar;
    public GameObject coinPrefab;
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
        Move(monoBehavior, transform, new Vector2(0, distance), timeSpan);
    }

    public static void Move(this MonoBehaviour monoBehavior, RectTransform transform, Vector2 change, float timeSpan)
    {
        var startPos = transform.anchoredPosition;

        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            transform.anchoredPosition = startPos + change * ratio;
        }));
    }

    public static void Move(this MonoBehaviour monoBehavior, Transform transform, Vector3 change, float timeSpan)
    {
        var startPos = transform.position;

        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            transform.position = startPos + change * ratio;
        }));
    }

    public static void MoveThenHide(this MonoBehaviour monoBehavior, Transform transform, Vector3 change, float timeSpan)
    {
        var startPos = transform.position;

        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            transform.position = startPos + change * ratio;

            if (ratio == 1)
            {
                //transform.gameObject.SetActive(false);
            }

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