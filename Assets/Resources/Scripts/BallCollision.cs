using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    static private BallCollision _inst;
    static public BallCollision Inst
    {
        get { return _inst; }
    }

    private Collision _collision;
    public Collision collision
    {
        get { return _collision; }
    }

    private void Awake()
    {
        _inst = this;
    }

    private void OnCollisionEnter (Collision other)
    {
        _collision = other;
    }

    public void OnCollisionExit (Collision other)
    {
        _collision = null;
    }
}
