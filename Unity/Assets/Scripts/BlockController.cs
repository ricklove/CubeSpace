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

    void Start()
    {
        var position = transform.Find("Position").gameObject;
        _frontBack = position.transform.Find("FrontBack").gameObject;
        _leftRight = position.transform.Find("LeftRight").gameObject;
        _topBottom = position.transform.Find("TopBottom").gameObject;
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
}
