using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Reopen
{
    [MenuItem("Tools/Reopen Project")]
    public static void ReopenProject()
    {
        EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }
}
