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
using System;

namespace Exp
{
    //Stocks the data to record
    //The methods starting with "AddColumn" create columns in the output array. These methods must be used one time at the beggining of the program
    //The methods starting with "Write" put the data to record. These methods "Write" must be called every frame with the new values.
    //These methods "Write" must be called between the methods NewLineRecording and EndLineRecording of the current trial.
    public class LData
    {
        //values of data
        internal Dictionary<string, string> _values = new Dictionary<string, string>();
        Dictionary<string, string> Values { get { return _values; } }

        //Constructor. Don't use it, it's automatically called by the module.
        public LData()
        {

        }

        //Initialize a new line. Don't use it, it's automatically called by the module.
        public void NewLine()
        {
            List<string> keys = new List<string>(_values.Keys);
            foreach (string k in keys)
            {
                _values[k] = "-";
            }
        }

        //Add column in the output array with name "name"
        public void AddColumn(string name)
        {
            _values.Add(name, "-");
        }

        //Add 7 columns with the name "name" to record a transformation matrix (pos_x, pos_y, pos_z, rot_x, rot_y, rot_z, rot_w)
        public void AddColumnsForMatrix(string name)
        {
            AddColumnsForPosition(name);
            AddColumnsForQuaternion(name);
        }

        //Add 3 columns with the name "name" to record a vector3 (x, y, z)
        public void AddColumnsForPosition(string name)
        {
            _values.Add(name + "_posx", "-");
            _values.Add(name + "_posy", "-");
            _values.Add(name + "_posz", "-");
        }

        //Add 4 columns with the name "name" to record a color (red, green, blue and alpha) 
        public void AddColumnsForColor(string name)
        {
            _values.Add(name + "_r", "-");
            _values.Add(name + "_g", "-");
            _values.Add(name + "_b", "-");
            _values.Add(name + "_a", "-");
        }

        //Add 4 columns with the name "name" to record a quaternion (x, y, z, w) 
        public void AddColumnsForQuaternion(string name)
        {
            _values.Add(name + "_rotx", "-");
            _values.Add(name + "_roty", "-");
            _values.Add(name + "_rotz", "-");
            _values.Add(name + "_rotw", "-");
        }

        //Write a matrix in the column "name"
        public void WriteMatrix(string name, Transform t)
        {
            WritePosition(name, t.position);
            WriteRotation(name, t.rotation);
        }

        //Write a quaternion in the column "name"
        public void WriteRotation(string name, Quaternion rot)
        {
            Write(name + "_rotx", rot.x.ToString());
            Write(name + "_roty", rot.y.ToString());
            Write(name + "_rotz", rot.z.ToString());
            Write(name + "_rotw", rot.w.ToString());
        }

        //Write a position in the column "name"
        public void WritePosition(string name, Vector3 pos)
        {
            Write(name + "_posx", pos.x.ToString());
            Write(name + "_posy", pos.y.ToString());
            Write(name + "_posz", pos.z.ToString());
        }

        //Write a color in the column "name"
        public void WriteColor(string name, Color color)
        {
            Write(name + "_r", color.r.ToString());
            Write(name + "_g", color.g.ToString());
            Write(name + "_b", color.b.ToString());
            Write(name + "_a", color.a.ToString());
        }

        //Write a value in the column "name"
        public void Write(string name, string value)
        {
            if (!_values.ContainsKey(name))
                throw new Exception("Data doesn't contain this name " + name);
            _values[name] = value;
        }
    }
}
