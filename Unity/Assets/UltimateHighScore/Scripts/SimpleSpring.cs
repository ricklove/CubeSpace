using UnityEngine;
using System.Collections;

public class SimpleSpring : MonoBehaviour
{
    public float slack = 0.1f;
    public float length = 1f;
    public float strength = 1f;
    public float maxForce = 10f;

    public GameObject[] attached;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (rigidbody == null) { return; }

        var minLength = length - slack;
        var minLengthSq = minLength * minLength;
        var maxLength = length + slack;
        var maxLengthSq = maxLength * maxLength;

        foreach (GameObject s in attached)
        {
            var diff = s.transform.position - transform.position;

            var force = diff.sqrMagnitude * strength;

            //force = Mathf.Min(maxForce, force);

            if (diff.sqrMagnitude > maxLengthSq)
            {
                // Go towards 
                rigidbody.AddForce(diff.normalized * force);
            }
            else if (diff.sqrMagnitude < minLengthSq)
            {
                // Go away
                rigidbody.AddForce(-diff.normalized * force);
            }
        }
    }
}
