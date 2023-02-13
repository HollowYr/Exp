#define DEBUG
using UnityEngine;
using System.Collections;
using System.IO;
using Directory = System.IO.Directory;
using File = System.IO.File;
using System.Collections.Generic;
using UnityEngine.Windows;

public static class SLS
{
    public static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public static void Init()
    {
        if (Directory.Exists(SAVE_FOLDER)) return;
        Directory.CreateDirectory(SAVE_FOLDER);
    }

    public static void Save(string saveString) =>
    File.WriteAllText(SAVE_FOLDER + "/save.txt", saveString);

    public static string Load()
    {
        if (!File.Exists(SAVE_FOLDER + "/save.txt")) return null;

        string saveString = File.ReadAllText(SAVE_FOLDER + "/save.txt");
        return saveString;
    }
}