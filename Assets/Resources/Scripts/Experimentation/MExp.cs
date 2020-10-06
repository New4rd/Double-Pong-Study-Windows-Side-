/////////////////////////////////////////////////////////////////////////////////////////
// 
//                 Université d'Aix Marseille (AMU) - 
//                 Centre National de la Recherche Scientifique (CNRS)
//                 Copyright © 2020 AMU, CNRS All Rights Reserved.
// 
//     These computer program listings and specifications, herein, are
//     the property of Université d'Aix Marseille and CNRS
//    shall not be reproduced or copied or used in whole or in part as
//     the basis for manufacture or sale of items without written permission.
//     For a license agreement, please contact:
//   <mailto: licensing@sattse.com> 
//
//
//
//    Author: Pergandi Jean-Marie - Laboratoire ISM - jean-marie.pergandi@univ-amu.fr
//
//////////////////////////////////////////////////////////////////////////////////////////
using System.IO;
using System;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace Exp
{
    //This class is the manager of the experimentation. It's a singleton and must be affected to a game object to use the experimentation module.
    //It gives access of all data of this module :
    //- the subject via MExp.inst.Subject
    //- the parameters of the experimentation via MExp.inst.Parameters
    //- the protocol of the experimentation via MExp.inst.Protocol
    //- the recording data of the experimentation via MExp.inst.Data

    public class MExp : MonoBehaviour
    {
        //Get the unique instance of this manager
        static MExp _inst;
        public static MExp Inst { get { return _inst; } }

        //The subject
        LSubject _subject;
        public LSubject Subject
        {
            get { return _subject; }
        }

        //The protocol
        LProtocol _protocol;
        public LProtocol Protocol
        {
            get { return _protocol; }
        }

        //The global parameters of the experimentation
        LParameters _parameters;
        public LParameters Parameters
        {
            get { return _parameters; }
        }

        //The data to record
        LData _data;
        public LData Data
        {
            get { return _data; }
        }

        //return the folder of the current subject 
        string _subjectDirectory;
        public string SubjectDirectory
        {
            get { return _subjectDirectory; }
        }

        //Return the recording folder
        string _recordingDirectory;
        public string RecordingDirectory
        {
            get { return _recordingDirectory; }
        }

        //Save or not the data
        bool _saveData = true;
        public bool SaveData { get { return _saveData; } }

        //this is the experimentation file. This file must be in the folder of the unity project or in the same folder as the executable
        //You have to give in Unity Editor the name of this file without the extension .txt
        public string _filename;

        void Awake()
        {
            _inst = this;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");//force the point . instead of comma ,
            
            //read the experimentation file
            string fullpath = Path.GetFullPath(_filename + ".txt");
            if (!File.Exists(fullpath))
                Debug.LogException(new NullReferenceException("File " + _filename + ".txt not found"));

            StreamReader stream = new StreamReader(fullpath);

            //read the folder of the subject
            string line = stream.ReadLine();
            string[] values = line.Split('\t');
            _subjectDirectory = values[1];

            //Read if we have to save data
            line = stream.ReadLine();
            values = line.Split('\t');
            _saveData = bool.Parse(values[1]);

            stream.Close();

            //check if the folder subject exists.
            _subjectDirectory = Path.GetFullPath(_subjectDirectory);
            if (!Directory.Exists(_subjectDirectory))
                Debug.LogException(new NullReferenceException("Folder " + _subjectDirectory + " not found"));

            //create the recording folder, if we have to save data
            if (_saveData)
            {
                _recordingDirectory = _subjectDirectory + "\\Session___" + DateTime.Now.ToString("yyyy_MM_dd___HH_mm");
                Directory.CreateDirectory(_recordingDirectory);
                _recordingDirectory = Path.GetFullPath(_recordingDirectory);
                if (!Directory.Exists(_recordingDirectory))
                    Debug.LogException(new NullReferenceException("Folder " + _recordingDirectory + " not found"));
            }

            //create subject, parameters, data and protocol
            _subject = new LSubject();
            _parameters = new LParameters();
            _data = new LData();
            _protocol = new LProtocol();
            Debug.Log("Experimentation initialized");
        }
    }
}