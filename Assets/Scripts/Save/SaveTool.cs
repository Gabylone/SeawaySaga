using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System;
using System.Text;


//tu peux changer le chemin de sauvegarde il y a troi ligne a changer :
//string path = Application.dataPath + dataPathSave;
public class SaveTool : MonoBehaviour
{
	public int chunkLimit = 10;

    public bool usePlayerPrefs = false;

	public static SaveTool Instance;

	void Awake () {
		Instance = this;
	}

	#region directories
	public void CreateDirectories ()
	{
        if(!DirectoryExists(GetSavePath() + "PlayerInfo"))
        {
            Directory.CreateDirectory(GetSavePath() + "PlayerInfo");
            Debug.Log("PlayerInfo directory DOESNT EXIST, creating it");
        }
        else
        {
            Debug.Log("PlayerInfo directory exists");
        }

		if ( DirectoryExists(GetCurrentMapPath()) == false ) {
//			Debug.Log ("BYTES SaveData folder doesnt exist, creating it");
			Directory.CreateDirectory (GetCurrentMapPath ());
		}
	}
	#endregion


	#region save & load
	public void ResetIslandFolder ()
	{
		if ( DirectoryExists(GetCurrentMapPath() + "/Islands") == true ) {
            Directory.Delete (GetCurrentMapPath() + "/Islands",true);
		}

		Directory.CreateDirectory (GetCurrentMapPath() + "/Islands");

	}

    public void DeleteFolder (string mapName)
    {
        Directory.Delete(GetSaveFolderPath(mapName), true);
    }

    public void DeleteGameData () {
		string path = GetCurrentMapPath () + "/game data.xml";
		File.Delete (path);
	}

    public void SaveToSpecificFolder (string folder , string path , object o)
    {
		path = GetSaveFolderPath (folder) + "/" + path + ".xml";
        Save(path, o);
    }

    public void SaveToCurrentMap ( string path , object o) {

		path = GetCurrentMapPath () + "/" + path + ".xml";
        Save(path, o);
    }

    public void Save (string path, object o)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(path);
        path = Encoding.Unicode.GetString(bytes);

        File.Delete(path);

        FileStream file = File.Open(path, FileMode.CreateNew);
        XmlSerializer serializer = new XmlSerializer(o.GetType());

        //		file = file.
        serializer.Serialize(file, o);

        file.Close();
    }

    public object LoadFromSpecificPath ( string mapName , string path , string className)
    {
        path = GetSaveFolderPath(mapName) + "/" + path;
        return LoadFromPath(path, className);
    }
	public object LoadFromCurrentMap(string path, string className)
	{
        path = GetCurrentMapPath() + "/" + path;
        return LoadFromPath(path, className);
	}
    public object LoadFromPath(string path, string className)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(path);
        path = Encoding.Unicode.GetString(bytes);

        //		FileStream file = File.Open(path, FileMode.OpenOrCreate);
        FileStream file = File.Open(path, FileMode.Open);
        XmlSerializer serializer = new XmlSerializer(Type.GetType(className));

        object o = serializer.Deserialize(file);

        file.Close();

        return o;
    }
    #endregion

    public bool FileExists(string mapName , string path)
    {
        if (usePlayerPrefs)
        {
            //PlayerPrefs.GetString("")
        }

        path = GetSaveFolderPath(mapName) + "/" + path + ".xml";

        byte[] bytes = Encoding.Unicode.GetBytes(path);
        path = Encoding.Unicode.GetString(bytes);

        bool exists = (File.Exists(path));

        return exists;
    }
	public bool DirectoryExists(string path)
	{
		byte[] bytes = Encoding.Unicode.GetBytes(path);
		path = Encoding.Unicode.GetString(bytes);

		bool exists = (Directory.Exists(path));

		return exists;
	}

    #region paths
    public string GetSaveFolderPath(string targetFolder)
    {

        string path = Application.dataPath + "/SaveData/" + targetFolder;

        if (Application.isMobilePlatform)
            path = Application.persistentDataPath + "/SaveData/" + targetFolder;

        return path;
    }
    public string GetCurrentMapPath () {

        string s = "";

        if ( KeepOnLoad.Instance != null && KeepOnLoad.Instance.mapName != "")
        {
            s = KeepOnLoad.Instance.mapName;
        }
        else
        {
            s = "Default";
        }

        string path = GetSavePath() + s;

		return path;
	}
    public string GetSavePath()
    {
        string path = Application.dataPath + "/SaveData/";

        if (Application.isMobilePlatform)
            path = Application.persistentDataPath + "/SaveData/";

        return path;
    }
	#endregion

}
