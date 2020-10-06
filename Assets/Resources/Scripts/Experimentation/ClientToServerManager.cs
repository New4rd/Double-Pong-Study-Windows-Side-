using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientToServerManager : NetworkBehaviour
{
    static private ClientToServerManager _inst;
    static public ClientToServerManager Inst
    {
        get { return _inst; }
    }


    public List<string> playerNames;


    private void Awake()
    {
        _inst = this;
    }
}
