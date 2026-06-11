using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace GFW
{
    public class Util 
    {
       
        public static string GetCallerFileDir([CallerFilePath] string path = "")
        {
            return Path.GetDirectoryName(path);
        }
        public static string GetCallerFilePath([CallerFilePath] string path = "")
        {
            return path;
        }
       
    }
}
