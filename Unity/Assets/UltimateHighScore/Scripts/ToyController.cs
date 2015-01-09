using UnityEngine;
using System.Collections;

public class ToyController : MonoBehaviour
{
    public delegate void ToyAction();

    public event ToyAction Exploded;

    public ParticleSystem _explosion;
    private bool _hasExplosionStarted = false;


    void Start()
    {
        //_explosion = transform.FindChild("Explosion").particleSystem;
    }

    void Update()
    {
        if (_explosion == null) { return; }

        if (_hasExplosionStarted)
        {
            if (_explosion.isStopped)
            {
                if (Exploded != null)
                {
                    Exploded();
                    _hasExplosionStarted = false;
                }
            }
        }
    }

    public void Explode()
    {
        if (_explosion == null) { return; }

        _explosion.Play();
        _hasExplosionStarted = true;
    }
}
