using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{
    public Vector3 _targetPosition;
    public Quaternion _targetRotation;

    private GameObject _frontBack;
    private GameObject _leftRight;
    private GameObject _topBottom;

    private float _lastWidth;
    private float _lastHeight;
    private float _lastDepth;

    private Shader _diffuse;
    private Shader _transDiffuse;

    void Awake()
    {
        var position = transform.Find("Position").gameObject;
        _frontBack = position.transform.Find("FrontBack").gameObject;
        _leftRight = position.transform.Find("LeftRight").gameObject;
        _topBottom = position.transform.Find("TopBottom").gameObject;

        _diffuse = Shader.Find("Diffuse");
        _transDiffuse = Shader.Find("Transparent/Diffuse");
    }

    void Update()
    {
        // Get size from local scale to adjust texture
        var width = transform.localScale.x;
        var height = transform.localScale.y;
        var depth = transform.localScale.z;

        if (width != _lastWidth
            || height != _lastHeight
            || depth != _lastDepth)
        {
            _frontBack.renderer.material.mainTextureScale = new Vector2(width, height);
            _leftRight.renderer.material.mainTextureScale = new Vector2(depth, height);
            _topBottom.renderer.material.mainTextureScale = new Vector2(width, depth);
        }

        _lastWidth = width;
        _lastHeight = height;
        _lastDepth = depth;
    }

    public void SetColor(Color color)
    {
        if (color.a < 1)
        {
            _frontBack.renderer.material.shader = _transDiffuse;
            _leftRight.renderer.material.shader = _transDiffuse;
            _topBottom.renderer.material.shader = _transDiffuse;
        }
        else
        {
            _frontBack.renderer.material.shader = _diffuse;
            _leftRight.renderer.material.shader = _diffuse;
            _topBottom.renderer.material.shader = _diffuse;
        }

        _frontBack.renderer.material.color = color;
        _leftRight.renderer.material.color = color;
        _topBottom.renderer.material.color = color;
    }
}
