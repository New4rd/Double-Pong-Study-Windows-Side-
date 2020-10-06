using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInstructionsManager : MonoBehaviour
{
    static private TextInstructionsManager _inst;
    static public TextInstructionsManager Inst
    {
        get { return _inst; }
    }


    private void Awake()
    {
        _inst = this;
    }
}
