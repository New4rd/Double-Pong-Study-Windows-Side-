using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exp;
using UnityEngine.UI;

/// <summary>
/// Manager pour le déroulé de l'expérimentation du Double Pong en collaboration VR.
/// </summary>

public class ExperimentationManager : MonoBehaviour
{

    /****************************************************************************************
     * **************************************************************************************
     *      I N S T A N C E   D U   M A N A G E R   D ' E X P E R I M E N T A T I O N
     * **************************************************************************************
     * *************************************************************************************/

    static private ExperimentationManager _inst;
    static public ExperimentationManager Inst
    {
        get { return _inst; }
    }


    /****************************************************************************************
     * **************************************************************************************
     *      V A R I A B L E S   P R I V E E S
     * **************************************************************************************
     * *************************************************************************************/

    /// <summary>
    /// Temps d'attente entre chaque message affiché sur le canvas.
    /// (valeur pouvant être directement incluse dans les paramètres d'expérimentation)
    /// </summary>
    private int secondsWait = 3;

    /// <summary>
    /// Booléen réglant l'enregistrement des données dans la fonction FixedUpdate.
    /// </summary>
    private bool writeDatas;

    /// <summary>
    /// Chaîne de caractères destinée à l'enregistrement des donnée. Lors de l'enregistrement, on
    /// indique en permanence qui a intercepté la balle (joueur 1, joueur 2 ou mur du fond)
    /// </summary>
    private string ballCatchedBy;

    /// <summary>
    /// Booléen indiquant si la phase actuelle est une phase d'entraînement.
    /// </summary>
    private bool trainingPhase = true;

    /// <summary>
    /// Booléen d'attente, vérifiant que les instructions se soient bien affichées avant le lancement
    /// du programme. (voir fonction TrainingDisplayText)
    /// </summary>
    private bool trainingDisplayTextDone;


    private bool _onePlayer;
    /// <summary>
    /// Booléen indiquant si l'expérimentation se fait à un joueur ou non.
    /// </summary>
    public bool onePlayer
    {
        get { return _onePlayer; }
    }


    /****************************************************************************************
     * **************************************************************************************
     *      F O N C T I O N   A W A K E
     * **************************************************************************************
     * *************************************************************************************/

    private void Awake()
    {
        _inst = this;
    }


    /****************************************************************************************
     * **************************************************************************************
     *      F O N C T I O N   S T A R T
     * **************************************************************************************
     * *************************************************************************************/

    private IEnumerator Start()
    {
        InitialSetup();
        yield return new WaitUntil(() => ServerSpawnManager.Inst.allSpawnCompleted);
        MExp.Inst.Protocol.StartExperimentation();
        SwitchTranspRackets();
        StartCoroutine(TrainingDisplayText());
        yield return new WaitUntil(() => trainingDisplayTextDone);
        StartCoroutine(Experimentation());
    }


    /// <summary>
    /// Fonction d'expérimentation, récursive. L'appeler lance l'expérimentation entière.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Experimentation ()
    {
        ballCatchedBy = "None";
        BallTrajectory.Inst.ResetInterpolations();
        ServerToClientManager.Inst.TranspColorChange(Color.gray);

        // Si on atteint le nombre d'essais d'entraînement, on passe en phase d'expérimentation
        if (MExp.Inst.Protocol.TrialIndex >= int.Parse(MExp.Inst.Parameters.Values["NumberOfTrainingTries"]))
            trainingPhase = false;

        // Si on est en phase d'entraînement, on affiche un message l'indiquant avant chaque lancer
        if (trainingPhase)
            EditDisplayText("Phase d'entraînement.");

        // Si on entre en phase d'expérimentation, on affiche un message l'indiquant une fois au début
        else if (MExp.Inst.Protocol.TrialIndex == int.Parse(MExp.Inst.Parameters.Values["NumberOfTrainingTries"])+1)
            EditDisplayText("Phase d'expérimentation.");

        // affichage des raquettes transparentes
        SwitchTranspRackets();

        EditDisplayText ("Veuillez positionner les raquettes sur les marqueurs transparents.");

        yield return new WaitUntil(() => CheckRacketPositionValidation());
        Debug.Log("SUPERPOSITION DETECTED");
        ServerToClientManager.Inst.TranspColorChange(Color.green);

        EditDisplayText("Lancement de la balle, essai n°" + MExp.Inst.Protocol.TrialIndex);
        /*
        // on attend 2 secondes, puis on fait disparaître les raquettes transparentes
        yield return new WaitForSecondsRealtime(2);*/
        SwitchTranspRackets();

        BallTrajectory.Inst.SetupTrajectory(MExp.Inst.Protocol.CurrentTrial.Variables["File_To_Load"]);

        MExp.Inst.Protocol.CurrentTrial.Start();
        writeDatas = !trainingPhase;    // on écrit des datas si on n'est pas en phase d'entraînement

        // on attend que la balle rencontre le mur du fond ou la raquette d'un des joueurs
        BallTrajectory.Inst.LaunchBall();
        yield return new WaitUntil(
            () => BallCollision.Inst.collision != null &&
            (BallCollision.Inst.collision.collider.tag == "Back Wall" || BallCollision.Inst.collision.collider.tag == "Racket"));

        BallTrajectory.Inst.moveball = false;
        ballCatchedBy = BallCollision.Inst.collision.collider.name;
        writeDatas = false;

        Debug.Log("BALL CATCHED BY::: " + ballCatchedBy);

        MExp.Inst.Protocol.CurrentTrial.End(true);
        ServerSpawnManager.Inst.ball.transform.position = new Vector3(0f, -10f, 0f);

        if (MExp.Inst.Protocol.NextTrial())
        {
            Debug.Log("NEXT TRY");
            StartCoroutine(Experimentation());
        }
        else
        {
            EditDisplayText("Fin de l'expérimentation, merci !");
            Debug.Log("END OF TRIES");
        }
    }


    public void InitialSetup()
    {
        _inst = this;
        _onePlayer = bool.Parse(MExp.Inst.Parameters.Values["OnePlayerExperimentation"]);
        BallTrajectory.Inst.SetupTrajectoryPath(MExp.Inst.SubjectDirectory + "\\Trajectories");
        ballCatchedBy = "None";

        // ajouter des colonnes de données
        // pour le suivi de position de la raquette 1
        MExp.Inst.Data.AddColumnsForPosition("Position Racket 1");
        if (!_onePlayer)
            // pour le suivi de position de la raquette 2
            MExp.Inst.Data.AddColumnsForPosition("Position Racket 2");
        MExp.Inst.Data.AddColumnsForPosition("Ball Position");
        // par qui la balle a-t-elle été attrapée ? (mur/raquette1/raquette2)
        MExp.Inst.Data.AddColumn("Ball Catched By");
    }


    private bool CheckRacketPositionValidation()
    {
        if (_onePlayer)
        {
            if (ServerSpawnManager.Inst.transparentRacketsList[0].GetComponent<DetectRacketCollision>().racketIsOnPosition)
            {
                return true;
            }
            
        }
        else
        {
            if (ServerSpawnManager.Inst.transparentRacketsList[0].GetComponent<DetectRacketCollision>().racketIsOnPosition
            && ServerSpawnManager.Inst.transparentRacketsList[1].GetComponent<DetectRacketCollision>().racketIsOnPosition)
            {
                return true;
            }
        }
        return false;
    }



    private void SwitchTranspRackets ()
    {
        for (int i = 0; i < ServerSpawnManager.Inst.transparentRacketsList.Count; i++)
        {
            if (ServerSpawnManager.Inst.transparentRacketsList[i].transform.position.y > 0)
            {
                ServerSpawnManager.Inst.transparentRacketsList[i].transform.position =
                new Vector3(
                    ServerSpawnManager.Inst.transparentRacketsList[i].transform.position.x,
                    -10,
                    ServerSpawnManager.Inst.transparentRacketsList[i].transform.position.z);
            }
            else
            {
                ServerSpawnManager.Inst.transparentRacketsList[i].transform.position =
                new Vector3(
                    ServerSpawnManager.Inst.transparentRacketsList[i].transform.position.x,
                    1.3f,
                    ServerSpawnManager.Inst.transparentRacketsList[i].transform.position.z);
            }
        }
    }


    private void EditDisplayText (string textToDisplay)
    {
        ServerSpawnManager.Inst.canvas.gameObject.transform.GetChild(0).GetComponent<Text>().text = textToDisplay;
        ServerToClientManager.Inst.textToDisplay = textToDisplay;
    }


    private IEnumerator TrainingDisplayText ()
    {
        trainingDisplayTextDone = false;
        EditDisplayText("Bienvenue dans cette expérience de coopération en VR.");
        yield return new WaitForSecondsRealtime(secondsWait);
        EditDisplayText("Utilisez le contrôleur pour déplacer la raquette, et atttraper la balle.");
        yield return new WaitForSecondsRealtime(secondsWait);
        EditDisplayText("Pour démarrer un essai, alignez la raquettes sur les marqueurs.");
        trainingDisplayTextDone = true;
    }


    
    private void FixedUpdate()
    {
        if (!writeDatas)
        {
            return;
        }

        Debug.Log("WRITING NEW LINE");
        MExp.Inst.Protocol.CurrentTrial.NewLineRecording();
        MExp.Inst.Data.WritePosition("Position Racket 1", ServerSpawnManager.Inst.racketsList[0].transform.position);
        if (!_onePlayer)
        {
            MExp.Inst.Data.WritePosition("Position Racket 2", ServerSpawnManager.Inst.racketsList[1].transform.position);
        }
        MExp.Inst.Data.WritePosition("Ball Position", ServerSpawnManager.Inst.ball.transform.position);
        MExp.Inst.Data.Write("Ball Catched By", ballCatchedBy);
        MExp.Inst.Protocol.CurrentTrial.EndLineRecording();
    }
    
}
