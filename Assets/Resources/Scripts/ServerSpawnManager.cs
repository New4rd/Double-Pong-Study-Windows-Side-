using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerSpawnManager : NetworkBehaviour
{
    static private ServerSpawnManager _inst;
    static public ServerSpawnManager Inst
    {
        get { return _inst; }
    }


    /************************************************************
     * **********************************************************
     *      G E T T E R     /   S E T T E R                     *
     * **********************************************************
     ************************************************************/

    private bool _allSpawnsCompleted;
    /// <summary>
    /// Booléen vérifiant si l'ensemble des opérations de spawn sur le serveur
    /// sont terminées.
    /// </summary>
    public bool allSpawnCompleted
    {
        get { return _allSpawnsCompleted; }
    }

    private List<GameObject> _transparentRacketsList;
    /// <summary>
    /// Liste des raquettes transparentes spawnées sur le serveur, servant à
    /// la vérification de la position du/des joueur(s) en début de manipulation.
    /// </summary>
    public List<GameObject> transparentRacketsList
    {
        get { return _transparentRacketsList; }
    }

    private List<GameObject> _racketsList;
    /// <summary>
    /// Liste des raquettes contrôlées par chacun des joueurs. Le joueur 1 contrôle
    /// la raquette d'indice 1 dans la liste, le joueur 2 contrôle la raquette d'indice 2
    /// dans la liste.
    /// </summary>
    public List<GameObject> racketsList
    {
        get { return _racketsList; }
    }

    private GameObject _ball;
    /// <summary>
    /// Objet de la balle.
    /// </summary>
    public GameObject ball
    {
        get { return _ball; }
    }


    private GameObject _canvas;
    /// <summary>
    /// Canvas utilisé pour l'affichage des instructions selon l'expérimentation.
    /// </summary>
    public GameObject canvas
    {
        get { return _canvas; }
    }


    private List<GameObject> _playerInfos;
    /// <summary>
    /// Liste de singletons contenant des informations relatives aux joueurs.
    /// </summary>
    public List<GameObject> playerInfos
    {
        get { return _playerInfos; }
    }


    private int nbPlayers;


    /************************************************************
     * **********************************************************
     *     A W A K E    F U N C T I O N                         *
     * **********************************************************
     ************************************************************/

    private void Awake()
    {
        _inst = this;
        _transparentRacketsList = new List<GameObject>();
        _racketsList = new List<GameObject>();
        _playerInfos = new List<GameObject>();
    }


    /************************************************************
     * **********************************************************
     *     S T A R T    F U N C T I O N                         *
     * **********************************************************
     ************************************************************/

    private IEnumerator Start()
    {
        switch (ExperimentationManager.Inst.onePlayer)
        {
            case true:  nbPlayers = 1; break;
            case false: nbPlayers = 2; break;
        }

        yield return new WaitUntil(() => NetworkServer.connections.Count > nbPlayers);
        for (int i = 1; i < nbPlayers+1; i++)
        {
            yield return new WaitUntil(() => NetworkServer.connections[i].isReady);
        }

        // Instanciation des informations relatives au joueur 1
        GameObject infoPrefab = (GameObject)Resources.Load("Prefabs/Player Informations");
        GameObject infos1 = Instantiate(infoPrefab, infoPrefab.transform);
        NetworkServer.SpawnWithClientAuthority(infos1, NetworkServer.connections[1]);
        _playerInfos.Add(infos1);

        // Instanciation du canvas pour l'affichage des instructions
        GameObject canvasPrefab = (GameObject)Resources.Load("Prefabs/Instructions Displayer");
        _canvas = Instantiate(canvasPrefab, canvasPrefab.transform);
        NetworkServer.Spawn(_canvas);

        // Instanciation de la balle
        GameObject ballPrefab = (GameObject)Resources.Load("Prefabs/Ball");
        _ball = Instantiate(ballPrefab, ballPrefab.transform);
        NetworkServer.Spawn(_ball);

        // Instanciation de la raquette rouge
        GameObject redRacketPrefab = (GameObject)Resources.Load("Prefabs/Red Racket");
        GameObject redRacketInstance = Instantiate(redRacketPrefab, redRacketPrefab.transform);
        NetworkServer.SpawnWithClientAuthority(redRacketInstance, NetworkServer.connections[1]);
        _racketsList.Add(redRacketInstance);

        // Instanciation d'une première raquette transparente
        GameObject transpRacketPrefab = (GameObject)Resources.Load("Prefabs/Transparent Racket");
        GameObject transpRacketInstance1 = Instantiate(transpRacketPrefab, new Vector3(2.5f, 1.3f, 1f), Quaternion.identity);
        _transparentRacketsList.Add(transpRacketInstance1);
        NetworkServer.Spawn(transpRacketInstance1);

        // Dans le cas de la présence de deux joueurs, on effectue des instanciations d'objets supplémentaires
        if (nbPlayers == 2)
        {
            // Instanciation des informations relatives au joueur 2
            GameObject infos2 = Instantiate(infoPrefab, infoPrefab.transform);
            NetworkServer.SpawnWithClientAuthority(infos2, NetworkServer.connections[2]);
            _playerInfos.Add(infos2);

            // Instanciation de la raquette bleue
            GameObject blueRacketPrefab = (GameObject)Resources.Load("Prefabs/Blue Racket");
            GameObject blueRacketInstance = Instantiate(blueRacketPrefab, blueRacketPrefab.transform);
            NetworkServer.SpawnWithClientAuthority(blueRacketInstance, NetworkServer.connections[2]);
            _racketsList.Add(blueRacketInstance);

            // Instanciation d'une seconde raquette transparente
            GameObject transpRacketInstance2 = Instantiate(transpRacketPrefab, new Vector3(2.5f, 1.3f, -1f), Quaternion.identity);
            _transparentRacketsList.Add(transpRacketInstance2);
            NetworkServer.Spawn(transpRacketInstance2);
        }

        _allSpawnsCompleted = true;
    }
}
