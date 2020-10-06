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
using System;
using System.Linq;

namespace Exp
{
    //Stock the parameters of the experimentation
    public class LParameters
    {
        //value of the parameters
        Dictionary<string, string> _values = new Dictionary<string, string>();
        public Dictionary<string, string> Values
        {
            get { return _values; }
        }

        //Constructor reads the Parameters.txt file. Don't use it, it's automatically called by the module.
        public LParameters()
        {
            if (!File.Exists(MExp.Inst.SubjectDirectory + "\\Parameters.txt"))
            {
                Debug.LogError(MExp.Inst.SubjectDirectory + "\\Parameters.txt not found");
                return;
            }
            StreamReader stream = new StreamReader(MExp.Inst.SubjectDirectory + "\\Parameters.txt");
            //read head
            while (!stream.EndOfStream)
            {
                string line = stream.ReadLine();
                string[] values = line.Split('\t');
                _values[values[0]] = values[1];
            }

            if (MExp.Inst.SaveData)
            {
                //Test reading
                StreamWriter outStream = new StreamWriter(MExp.Inst.RecordingDirectory + "//outParameters.txt");
                for (int i = 0; i < _values.Count; i++)
                {
                    if (i < _values.Count - 1)
                        outStream.WriteLine(_values.ElementAt(i).Key + "\t" + _values.ElementAt(i).Value);//end of line
                    else
                        outStream.Write(_values.ElementAt(i).Key + "\t" + _values.ElementAt(i).Value);//end of file
                }
                outStream.Close();

                //Compare
                {
                    byte[] inContents = File.ReadAllBytes(MExp.Inst.SubjectDirectory + "\\Parameters.txt");
                    byte[] outContents = File.ReadAllBytes(MExp.Inst.RecordingDirectory + "\\outParameters.txt");
                    if (!inContents.SequenceEqual(outContents))
                        throw new Exception("The two files Parameters.txt and outParameters.txt are differents!");
                }
            }
        }
    }
}