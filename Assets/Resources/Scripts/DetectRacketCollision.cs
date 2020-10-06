using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DetectRacketCollision : NetworkBehaviour
{
    private bool _racketIsOnPosition;
    public bool racketIsOnPosition
    {
        get { return _racketIsOnPosition; }
    }

    public float superpositionThreshold = .1f;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.tag == "Racket")
        {
            yield return new WaitUntil(() => Mathf.Abs(other.transform.position.z - transform.position.z) < superpositionThreshold);
            _racketIsOnPosition = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Racket")
        {
            _racketIsOnPosition = false;
        }
    }
}
