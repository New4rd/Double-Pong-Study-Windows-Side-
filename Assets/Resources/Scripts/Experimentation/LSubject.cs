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
using System.IO;
using System;
using System.Linq;
using UnityEngine;

namespace Exp
{
    //Manage the data of the subject
    public class LSubject
    {
        //Stock the attributes of the subject declared in the Subject.txt file
        Dictionary<string, string> _attributes = new Dictionary<string, string>();
        public Dictionary<string, string> Attributes { get { return _attributes; } }

        //Constructor read the Subject.txt file and initialize the attributes of the class Subject. Don't use it, it's automatically called by the module.
        public LSubject()
        {
            if (!File.Exists(MExp.Inst.SubjectDirectory + "\\Subject.txt"))
            {
                Debug.LogError(MExp.Inst.SubjectDirectory + "\\Subject.txt not found");
                return;
            }
            StreamReader stream = new StreamReader(MExp.Inst.SubjectDirectory + "\\Subject.txt");

            while (!stream.EndOfStream)
            {
                string line = stream.ReadLine();
                string[] values = line.Split('\t');
                _attributes[values[0]] = values[1];
            }
            stream.Close();

            if (MExp.Inst.SaveData)
            {

                //Test reading
                StreamWriter outStream = new StreamWriter(MExp.Inst.RecordingDirectory + "//outSubject.txt");
                for (int i = 0; i < _attributes.Count; i++)
                {
                    if (i < _attributes.Count - 1)
                        outStream.WriteLine(_attributes.ElementAt(i).Key + "\t" + _attributes.ElementAt(i).Value);//end of line
                    else
                        outStream.Write(_attributes.ElementAt(i).Key + "\t" + _attributes.ElementAt(i).Value);//end of file
                }
                outStream.Close();
                //Compare
                {
                    byte[] inContents = File.ReadAllBytes(MExp.Inst.SubjectDirectory + "\\Subject.txt");
                    byte[] outContents = File.ReadAllBytes(MExp.Inst.RecordingDirectory + "\\outSubject.txt");
                    if (!inContents.SequenceEqual(outContents))
                        throw new Exception("The two files Subject.txt and outSubject.txt are differents!");
                }
            }
        }
    }
}