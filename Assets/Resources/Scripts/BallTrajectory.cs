using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// https://fr.wikipedia.org/wiki/%C3%89quation_de_droite

public class BallTrajectory : MonoBehaviour
{
    static private BallTrajectory _inst;
    static public BallTrajectory Inst
    {
        get { return _inst; }
    }

    private Vector3 startPoint;
    private Vector3 endPoint;
    public float speed;

    private List<Vector3> interpolations;
    private bool generationDone = false;
    private int listPos = 0;

    private string path;

    private bool _moveBall;
    public bool moveball
    {
        get { return _moveBall; }
        set { _moveBall = value; }
    }

    private void Awake()
    {
        _inst = this;
        _moveBall = false;
        interpolations = new List<Vector3>();
    }


    private void FixedUpdate()
    {
        if (!_moveBall) return;

        if (listPos < interpolations.Count)
        {
            ServerSpawnManager.Inst.ball.transform.position = interpolations[listPos];
            //BallManager.Inst.SetBallPosition(interpolations[listPos]);
            listPos++;
        }

        if (listPos == interpolations.Count)
        {
            _moveBall = false;
        }
        
    }


    public void ResetInterpolations()
    {
        listPos = 0;
        interpolations = new List<Vector3>();
    }


    public void SetupTrajectoryPath (string filePath)
    {
        path = filePath;
    }


    StreamReader sr;
    private void SetFileReader (string filename)
    {
        sr = new StreamReader(path + "\\" + filename);
    }


    private void SetInterpolations ()
    {
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] stringCoords = line.Split('\t');
            interpolations.Add(new Vector3(
                float.Parse(stringCoords[0]),
                float.Parse(stringCoords[1]),
                float.Parse(stringCoords[2])));
        }
    }


    public void SetupTrajectory (string filename)
    {
        ResetInterpolations();
        SetFileReader(filename);
        SetInterpolations();
    }


    public void LaunchBall ()
    {
        _moveBall = true;
    }
}
