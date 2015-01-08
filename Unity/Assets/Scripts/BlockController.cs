using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{
    public Vector3 _targetPosition;
    public Quaternion _targetRotation;

    private GameObject _front;
    private GameObject _back;
    private GameObject _left;
    private GameObject _right;
    private GameObject _top;
    private GameObject _bottom;


    private float _lastWidth;
    private float _lastHeight;
    private float _lastDepth;

    private Shader _diffuse;
    private Shader _transDiffuse;

    void Awake()
    {
        var position = transform.Find("Position").gameObject;
        _front = position.transform.Find("Front").gameObject;
        _back = position.transform.Find("Back").gameObject;
        _left = position.transform.Find("Left").gameObject;
        _right = position.transform.Find("Right").gameObject;
        _top = position.transform.Find("Top").gameObject;
        _bottom = position.transform.Find("Bottom").gameObject;

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
            _front.renderer.material.mainTextureScale = new Vector2(width, height);
            _left.renderer.material.mainTextureScale = new Vector2(height, depth);
            _top.renderer.material.mainTextureScale = new Vector2(width, depth);

            _back.renderer.material.mainTextureScale = new Vector2(width, height);
            _right.renderer.material.mainTextureScale = new Vector2(height, depth);
            _bottom.renderer.material.mainTextureScale = new Vector2(width, depth);
        }

        _lastWidth = width;
        _lastHeight = height;
        _lastDepth = depth;
    }

    public void SetColor(Color color)
    {
        if (color.a < 1)
        {
            _front.renderer.material.shader = _transDiffuse;
            _back.renderer.material.shader = _transDiffuse;
            _left.renderer.material.shader = _transDiffuse;
            _right.renderer.material.shader = _transDiffuse;
            _top.renderer.material.shader = _transDiffuse;
            _bottom.renderer.material.shader = _transDiffuse;
        }
        else
        {
            _front.renderer.material.shader = _diffuse;
            _back.renderer.material.shader = _diffuse;
            _left.renderer.material.shader = _diffuse;
            _right.renderer.material.shader = _diffuse;
            _top.renderer.material.shader = _diffuse;
            _bottom.renderer.material.shader = _diffuse;
        }

        _front.renderer.material.color = color;
        _back.renderer.material.color = color;
        _left.renderer.material.color = color;
        _right.renderer.material.color = color;
        _top.renderer.material.color = color;
        _bottom.renderer.material.color = color;
    }
}
