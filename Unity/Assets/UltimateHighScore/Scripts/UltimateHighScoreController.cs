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
    //private GameObject _messages;
    private Vector2 _messageScreenSize;

    private GameObject _coins;
    private ParticleSystem _coinFlare;


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
        //_messages = _messageCanvas.transform.FindChild("Messages").gameObject;
        _messageProto = _messageCanvas.transform.FindChild("MessageProto").gameObject;
        _messageProto.SetActive(false);

        _coins = transform.FindChild("Coins").gameObject;
        _coinFlare = _coins.transform.FindChild("Flare").FindChild("Flare").GetComponent<ParticleSystem>();

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

    public void ResetScore(string id, float startScore)
    {
        var sData = GetScoreData(id);
        sData.score = startScore;
        sData.text.text = "" + sData.score;
        sData.combo = 0;
        AdjustCombo(sData, Color.green);
    }

    public void ResetCombo(string id)
    {
        var sData = GetScoreData(id);
        sData.combo = 0;
        AdjustCombo(sData, Color.green);
    }


    private float? _timeAtLastScore = null;

    public void AddScore(string id, float scoreChange, Vector3 worldPosition, Color color)
    {
        var sData = GetScoreData(id);

        AdjustCombo(sData, color);


        // Adjust score
        var oldScore = sData.score;

        sData.score += scoreChange * sData.combo;

        // Show Score message
        var mObj = (GameObject)Instantiate(_messageProto);
        mObj.transform.SetParent(_messageProto.transform.parent, false);
        //mObj.transform.position = _messageProto.transform.position;
        //mObj.transform.rotation = _messageProto.transform.rotation;
        //mObj.transform.localScale = _messageProto.transform.localScale;

        var mRect = mObj.GetComponent<RectTransform>();
        var uiPos = WorldToCanvasPosition(GetCanvas(mRect), worldPosition);

        var mText = mObj.GetComponent<Text>();
        mText.text = "+" + scoreChange;

        mRect.anchoredPosition = uiPos + new Vector2(0, Screen.height * 0.1f);

        var timeToShowMessage = 0.75f;
        this.FadeOut(mText, timeToShowMessage);
        this.FloatUp(mRect, Screen.height * 0.2f, timeToShowMessage);
        mRect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.Scale(mRect, new Vector3(1, 1, 1), timeToShowMessage);

        //mObj.transform.position = worldPosition;
        //this.Move(mObj.transform, new Vector3(0, 5f, 0), 2f);

        Destroy(mObj, 5);

        mObj.SetActive(true);

        // Show coins
        Debug.Log("sData.coinPrefab=" + sData.coinPrefab);

        if (sData.coinPrefab != null)
        {
            var fromPos = worldPosition;
            var toPos = RectToWorldPosition(sData.text.rectTransform);

            //var diff = toPos - fromPos;
            //var toPastPos = fromPos + toPos * 2;
            //toPos = toPastPos;


            var coinCount = scoreChange;
            var timeToShowCoins = 1.0f;
            var maxTimePerCoin = 0.2f;
            var timeToMove = 0.5f;
            var timeToDeflateScoreText = 3f;
            var maxCoins = 11;

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

                StartCoroutine(CreateCoin(sData.coinPrefab, color, sData.text, scoreTextAfterChange,
                    fromPos, toPos, timeToMove, i * timePerCoin));

            }

            _coinFlare.Play();
            StartCoroutine(StopFlare(timeToShowCoins));

            // Show score in text
            var oldScale = new Vector3(1, 1, 1);//sData.text.transform.localScale;
            this.Scale(sData.text.transform, oldScale, timeToDeflateScoreText);
        }
        else
        {
            // Show score in text
            sData.text.text = "" + sData.score;
        }

        sData.text.color = (color * 0.75f) + new Color(0.25f, 0.25f, 0.25f, 1);


        _timeAtLastScore = Time.time;
    }

    private IEnumerator StopFlare(float timeToShowCoins)
    {
        yield return new WaitForSeconds(timeToShowCoins);
        _coinFlare.Stop();
    }

    private void AdjustCombo(ScoreData sData, Color color)
    {
        // Adjust combo
        if (sData.comboBar != null)
        {
            if (_timeAtLastScore != null && Time.time - _timeAtLastScore < sData.comboTimeout)
            {
                sData.combo++;
            }
            else
            {
                sData.combo = 1;
            }

            if (sData.comboText != null)
            {
                sData.comboText.text = "Combo\r\nx"
                    + sData.combo;

                if (sData.combo == 1)
                {
                    this.Scale(sData.comboText.transform, new Vector3(1, 1, 1), 1.5f);
                    this.FadeOut(sData.comboText, 0);
                }
                else
                {
                    //sData.comboText.transform.localScale *= 1.5f;
                    var scale = 1 + 0.25f * Mathf.Log(sData.combo);
                    this.Scale(sData.comboText.transform, new Vector3(scale * 1.5f, scale * 1.5f, scale * 1.5f), 0.25f);
                    this.Scale(sData.comboText.transform, new Vector3(scale, scale, scale), 0.5f);
                    this.FadeOut(sData.comboText, 1f);
                }

            }

            // Show combo toys
            if (sData.comboToyProto != null && sData.comboToyContainer != null)
            {
                sData.comboToyProto.SetActive(false);

                while (sData.comboToyContainer.transform.childCount < sData.combo)
                {
                    // Create Combo toy
                    var cToy = (GameObject)Instantiate(sData.comboToyProto);
                    cToy.transform.SetParent(sData.comboToyContainer.transform, true);
                    cToy.transform.localPosition = new Vector3();
                    cToy.transform.localScale = sData.comboToyProto.transform.localScale;
                }

                var i = 0;
                foreach (Transform c in sData.comboToyContainer.transform)
                {
                    if (i < sData.combo)
                    {
                        if (!c.gameObject.activeSelf)
                        {
                            c.gameObject.SetActive(true);
                            StartCoroutine(SetColorAfterDelay(c.GetComponent<ToyController>(), color, 0.5f));
                        }

                    }
                    else
                    {
                        StartCoroutine(ExplodeToyAfterDelay(c, (i - sData.combo) * 0.1f));
                    }

                    i++;
                }
            }
        }
    }

    private IEnumerator SetColorAfterDelay(ToyController toyController, Color color, float delay)
    {
        yield return new WaitForSeconds(delay);
        toyController.SetColor(color);
    }

    private static IEnumerator ExplodeToyAfterDelay(Transform c, float delay)
    {
        yield return new WaitForEndOfFrame();
        c.transform.parent = c.transform.root;

        yield return new WaitForSeconds(delay);

        // TODO: Explode combo toys
        //c.gameObject.SetActive(false);
        var toy = c.gameObject.GetComponent<ToyController>();
        ToyController.ToyAction action = null;

        action = () =>
        {
            toy.gameObject.SetActive(false);
            toy.Exploded -= action;
        };

        toy.Exploded += action;
        toy.Explode();

        yield return new WaitForSeconds(0.25f);
        c.renderer.active = false;

        yield return new WaitForSeconds(5f);
        Destroy(c.gameObject);
    }





    private IEnumerator CreateCoin(GameObject coinPrefab, Color color, Text score, string scoreTextAfterChange, Vector3 startPos, Vector3 endPos, float timeToMove, float delay)
    {
        yield return new WaitForSeconds(Random.RandomRange(0.01f, 0.1f));
        yield return new WaitForSeconds(delay);

        //Debug.Log("CreateCoin");


        var obj = (GameObject)Instantiate(coinPrefab);
        var transform = obj.transform;
        transform.parent = _coins.transform;
        transform.position = startPos;

        obj.GetComponent<BlockController>().SetColor(color);

        obj.SetActive(true);

        this.Move(transform, endPos - startPos, timeToMove);
        //this.Rotate(transform, new Vector3(720, 180, 1080), timeToMove);
        this.Rotate(transform, new Vector3(10, 180, 60), timeToMove);

        yield return new WaitForSeconds(timeToMove);
        obj.SetActive(false);
        Destroy(obj, timeToMove + 0.1f);

        score.text = scoreTextAfterChange;

        // Scale up text
        score.transform.localScale *= 1.05f;
    }



    //private Vector2 WorldToUIPosition(Vector3 worldPosition)
    //{
    //    var cam = Camera.mainCamera;

    //    var screenPos = cam.WorldToScreenPoint(worldPosition);
    //    var screenRatio = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
    //    var uiPos = new Vector2(screenRatio.x * _messageScreenSize.x, screenRatio.y * _messageScreenSize.y);

    //    return uiPos;
    //}

    //private Vector2 UIToWorldPosition(Vector3 uiPosition)
    //{
    //    var cam = Camera.mainCamera;

    //    var screenRatio = new Vector2(uiPosition.x / _messageScreenSize.x, uiPosition.y / _messageScreenSize.y);
    //    var screenPos = new Vector2(screenRatio.x * Screen.width, screenRatio.y * Screen.height);
    //    var worldPos = cam.ScreenToWorldPoint(screenPos);

    //    return worldPos;
    //}

    private Vector2 WorldToCanvasPosition(Canvas canvas, Vector3 worldPosition)
    {
        var cam = Camera.mainCamera;

        var screenPos = cam.WorldToScreenPoint(worldPosition);
        var screenRatio = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);

        var targetSize = canvas.GetComponent<CanvasScaler>().referenceResolution;
        var actualScaledSize = new Vector2(Screen.width / canvas.scaleFactor, Screen.height / canvas.scaleFactor);
        var xScale = actualScaledSize.x / targetSize.x;
        var yScale = actualScaledSize.y / targetSize.y;

        var proportionalSize = Vector2.Scale(targetSize, new Vector2(xScale, yScale));
        var size = proportionalSize;


        Debug.Log("screenPos:" + screenPos);
        Debug.Log("screenRatio:" + screenRatio);
        Debug.Log("scaleFactor:" + canvas.scaleFactor);
        Debug.Log("targetSize:" + targetSize);
        Debug.Log("actualScaledSize:" + actualScaledSize);
        Debug.Log("proportionalSize:" + proportionalSize);

        return Vector2.Scale(screenRatio, size);
    }

    private Vector2 WorldToRectPosition(RectTransform transform, Vector3 worldPosition)
    {
        var cam = Camera.mainCamera;

        var screenPos = cam.WorldToScreenPoint(worldPosition);
        var screenRect = GetScreenRect(transform);
        var screenRatio = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);

        Debug.Log("screenPos:" + screenPos);
        Debug.Log("screenRect:" + screenRect);
        Debug.Log("screenRatio:" + screenRatio);

        return screenRect.min + Vector2.Scale(screenRect.size, screenRatio);

        //var uiPos = new Vector2(screenRatio.x * _messageScreenSize.x, screenRatio.y * _messageScreenSize.y);
        //return uiPos;
    }

    private Vector3 RectToWorldPosition(RectTransform transform)
    {
        var cam = Camera.mainCamera;

        var screenRect = GetScreenRect(transform);
        var screenPos = screenRect.center;

        // TODO: find out where center of rect is in world coordinates
        // Assume rect is at center of world
        var toPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, (cam.transform.position - Vector3.zero).magnitude - 1));
        //var toPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 3f));


        //Debug.Log("sData.text.transform.position=" + transform.position);
        //Debug.Log("screenPos=" + screenPos);
        //Debug.Log("toPos=" + toPos);

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

            Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(rectTransform), corners[i]);

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

    private static Camera GetCanvasCamera(RectTransform rectTransform)
    {
        var canvas = GetCanvas(rectTransform);

        if (canvas != null) { return canvas.worldCamera; }

        return null;
    }

    private static Canvas GetCanvas(RectTransform rectTransform)
    {
        Transform r = rectTransform;
        while (r.GetComponent<Canvas>() == null)
        {
            if (r.parent == null) { return null; }

            r = r.parent;
        }

        return r.GetComponent<Canvas>();
    }






}

[System.Serializable]
public class ScoreData
{
    public string id;
    public float score;
    public float combo;
    public Text text;
    public GameObject coinPrefab;
    public GameObject comboBar;
    public Text comboText;
    public GameObject comboToyContainer;
    public GameObject comboToyProto;
    public float comboTimeout = 10f;
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

    public static void Rotate(this MonoBehaviour monoBehavior, Transform transform, Vector3 totalAngels, float timeSpan)
    {
        var lastRatio = 0f;
        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            var ratioChange = ratio - lastRatio;
            lastRatio = ratio;

            transform.Rotate(totalAngels * ratioChange);
        }));
    }


    public static void Scale(this MonoBehaviour monoBehavior, Transform transform, Vector3 target, float timeSpan)
    {
        var lastRatio = 0f;
        monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
        {
            if (ratio == 1)
            {
                transform.localScale = target;
                return;
            }

            var diff = target - transform.localScale;
            var remainingRatio = 1.0f - ratio;

            var ratioChange = ratio - lastRatio;
            lastRatio = ratio;
            var ticksRemaining = remainingRatio / ratioChange;

            transform.localScale = transform.localScale + diff / ticksRemaining;

        }));
    }


    //public static void MoveThenHide(this MonoBehaviour monoBehavior, Transform transform, Vector3 change, float timeSpan)
    //{
    //    var startPos = transform.position;

    //    monoBehavior.StartCoroutine(DoOverTime(timeSpan, (ratio) =>
    //    {
    //        transform.position = startPos + change * ratio;

    //        if (ratio == 1)
    //        {
    //            //transform.gameObject.SetActive(false);
    //        }

    //    }));
    //}


    private static IEnumerator DoOverTime(float duration, TransitionAction doAction)
    {
        if (duration <= 0)
        {
            doAction(1);
            yield break;
        }

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