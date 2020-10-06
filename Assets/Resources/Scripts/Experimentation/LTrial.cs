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
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Exp
{
    //Represents a trial

    //delegate to receive event
    public delegate void TrialStarted();
    //delegate to receive event
    public delegate void TrialEnded(bool success);

    public class LTrial
    {
        //get the variables of the trial declared in the Protocol.txt file
        Dictionary<string, string> _variables = new Dictionary<string, string>();
        public Dictionary<string, string> Variables { get { return _variables; } }

        //index of this trial in the protocol file
        int _index = -1;
        public int Index { get { return _index; } }

        //order played in the experimentation (a trial can failed and be replayed later)
        int _order = -1;
        public int Order
        {
            get { return _order; }
            set { _order = value; }//Don't use it, it's automatically called by the module.
        }

        /****************MANAGEMENT OF RECORDING FILE*************************/
        //the recording file of this trial (there's one file by trial)
        string _filename;

        public string Filename
        {
            get
            {
                if (_success)
                    return AddressBase + ".txt";
                return AddressBase + "_" + MExp.Inst.Protocol.ReplayFailedTrialType.ToString() + ".txt";
            }
        }
        string AddressBase
        {
            get
            {
                return MExp.Inst.RecordingDirectory + "\\Trial_Order" + _order + "_Id" + _index;
            }
        }

        //stream to sava data in the recording file
        StreamWriter _stream;

        public void NewLineRecording()
        {
            MExp.Inst.Data.NewLine();
        }

        public void EndLineRecording()
        {
            if (_startChrono == -1)//at the first call of _Run we start the chrono now (the first record starts with a chrono at 0)
                _startChrono = Time.realtimeSinceStartup;
            if (MExp.Inst.SaveData)//write data
            {
                _stream.Write(Time.realtimeSinceStartup + "\t" + Chrono);
                foreach (string key in MExp.Inst.Data._values.Keys)
                    _stream.Write("\t" + MExp.Inst.Data._values[key]);
                _stream.WriteLine();
                _stream.Flush();
            }
        }

        /**********MANAGEMENT OF FAILURE**********************/
        //the trial is right or failed?
        bool _success = true;
        public bool Success
        {
            get { return _success; }
        }

        //Change the name of the file to say that this trial is failed. Don't use it, it's automatically called by the module.
        void MarkAsFailed()
        {
            if (MExp.Inst.SaveData)
            {
                File.Move(AddressBase + ".txt", AddressBase + "_" + MExp.Inst.Protocol.ReplayFailedTrialType.ToString() + ".txt");
            }
        }

        //Reset the state of the trial. Don't use it, it's automatically called by the module.
        internal void ResetState()
        {
            _state = TrialState.NotStarted;
        }

        /***************MANAGEMENT OF THE TRIAL ****************/

        //chrono of the trial
        float _startChrono = -1;//the trial doesn't start
        public float Chrono
        {
            get
            {
                if (_startChrono == -1)
                    return 0;
                return Time.realtimeSinceStartup - _startChrono;
            }
        }

        //State of the trial (not started, or running, or ended)
        TrialState _state = TrialState.NotStarted;
        public TrialState State
        {
            get { return _state; }
        }

        //start the trial
        public void Start()
        {
            if (_state != TrialState.NotStarted)
                return;
            _startChrono = -1;
            _success = true;

            if (MExp.Inst.SaveData)//write head
            {
                _stream = new StreamWriter(Filename);
                foreach (KeyValuePair<string, string> pair in _variables)
                    _stream.WriteLine(pair.Key + "\t" + pair.Value);
                _stream.Write("SessionTime\tTrialTime");
                foreach (string key in MExp.Inst.Data._values.Keys)
                    _stream.Write("\t" + key);
                _stream.WriteLine();
            }
            _state = TrialState.Running;
            if (onTrialStarted != null)
                onTrialStarted();
        }

        //end the trial
        public void End(bool success)
        {
            if (_state != TrialState.Running)
                return;
            if (MExp.Inst.SaveData)
            {
                _stream.Write(Time.realtimeSinceStartup + "\t" + (Time.realtimeSinceStartup - _startChrono).ToString());
                foreach (string key in MExp.Inst.Data._values.Keys)
                    _stream.Write("\t" + MExp.Inst.Data._values[key]);
                _stream.WriteLine();
                _stream.Close();
            }
            _state = TrialState.Ended;
            _success = success;
            if (!_success)
                MarkAsFailed();
            if (onTrialEnded != null)
                onTrialEnded(_success);
        }

        /********************EVENT*****************/
        public TrialStarted onTrialStarted;
        public TrialEnded onTrialEnded;

        //Constructor. Don't use it, it's automatically called by the module.
        public LTrial(int index, List<string> head, string[] values)
        {
            _index = index;
            for (int i = 0; i < values.Length; i++)
                _variables[head[i]] = values[i];
        }
    }

    public enum TrialState
    {
        NotStarted,
        Running,
        Ended
    }
}