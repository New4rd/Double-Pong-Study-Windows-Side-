using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ServerToClientManager : NetworkBehaviour
{
    static private ServerToClientManager _inst;
    static public ServerToClientManager Inst
    {
        get { return _inst; }
    }

    private Material transpMaterial;

    [SyncVar]
    public Color transpColor;

    [SyncVar]
    public string textToDisplay;

    private void Awake()
    {
        _inst = this;
        transpMaterial = (Material)Resources.Load("Materials/TransparentRacketMaterial");
        transpMaterial.color = Color.gray;
    }
    
    public void TranspColorChange (Color colorChange)
    {
        transpMaterial.color = colorChange;
        transpColor = colorChange;
    }
}
