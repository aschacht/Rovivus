// PlayUp Tools - www.playuptools.com

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;

using System;
using System.Xml;

public class PlayUp: EditorWindow {

	string myString = "";
	
	[MenuItem ("PlayUp/Import Level")]
	
	static void  Init () {
		// Get existing open window or if none, make a new one:
		PlayUp window = (PlayUp)EditorWindow.GetWindow (typeof (PlayUp));
		window.Show ();
	}
	
	void OnGUI  () {
		Texture2D texty = Resources.LoadAssetAtPath("Assets/Editor PlayUp Resources/playup-logo-unity.png", typeof(Texture2D)) as Texture2D; 
		if (texty) GUI.DrawTexture(new Rect(20,0,227, 60), texty); 	
		GUI.BeginGroup (new  Rect (10, 70, 270, 200));
		
		GUILayout.BeginHorizontal (GUILayout.Width(200));
		GUILayout.Space (50);
		if(GUILayout.Button("www.playuptools.com", GUILayout.Width(170))) {
			Application.OpenURL("http://www.playuptools.com");
		}
		GUILayout.EndHorizontal ();
		GUILayout.Space (10);
		if (myString != "") {
			GUILayout.Label ("LEVEL SELECTED FOR IMPORT: ", EditorStyles.boldLabel);
			GUILayout.Label (myString, EditorStyles.boldLabel);
			GUILayout.Label ("");
			GUILayout.Label ("Click on the Import button to load the level.");
		}
		else {
			GUILayout.Label ("LEVEL SELECTED FOR IMPORT: ", EditorStyles.boldLabel);
			GUILayout.Label ("none", EditorStyles.boldLabel);
			GUILayout.Label ("");
			GUILayout.Label ("Select a level file by clicking the Browse button.");
		}
		GUILayout.BeginHorizontal (GUILayout.Width(200));
	   if(GUILayout.Button("Browse", GUILayout.Width(100))) {
		   myString = Path.GetFileName(EditorUtility.OpenFilePanel("Choose the Level File", "Assets/PlayUp/Levels/", "lvl"));
	   } 
	   if (myString != "") {
		   if(GUILayout.Button("Import", GUILayout.Width(100))) {
			   if (myString != "") FiletoObj(myString);
			   else Debug.Log("You have not yet selected a level file.  Please select a level file by clicking the Browse button.");
		   } 
		}
	GUILayout.EndHorizontal ();
		GUI.EndGroup ();
	}
	
    public static void FiletoObj(string levelName)
    {
	
		char[] delimiterChars = { ',' };
		GameObject level = new GameObject(Path.GetFileNameWithoutExtension(levelName));
    	
		//Create the XmlDocument.
		XmlDocument doc = new XmlDocument();
		string path = "Assets\\PlayUp\\Levels\\" + levelName;
		FileStream fs = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
		doc.Load(fs);
		XmlNodeList node = doc.GetElementsByTagName("OBJECT");
		for (int i=0; i<node.Count; i++)
		{
			GameObject prefab = null;
			GameObject clone = null;
			XmlAttributeCollection attrc = node[i].Attributes;
			for (int j=0; j<attrc.Count; j++)
			{
				if (attrc[j].Name == "NAME"){
					prefab = Resources.LoadAssetAtPath("Assets/PlayUp/Objects/" + attrc[j].Value + ".dae", typeof(GameObject)) as GameObject; 
					clone = EditorUtility.InstantiatePrefab(prefab) as GameObject;
				}
				if (attrc[j].Name == "POSITION"){
					string[] vals = attrc[j].Value.Split(delimiterChars);
					Vector3 pos;
					pos.x = float.Parse(vals[0]);
					pos.z = float.Parse(vals[1]);
					pos.y = float.Parse(vals[2]);
					clone.transform.position = pos;
				}
				if (attrc[j].Name == "ROTATION"){
					string[] vals = attrc[j].Value.Split(delimiterChars);
					Vector3 rot;
					rot.x = float.Parse(vals[0])-90;
					rot.y = float.Parse(vals[1]);
					rot.z = float.Parse(vals[2])+180;
					clone.transform.eulerAngles = rot;
				}
				if (attrc[j].Name == "SCALE"){
					string[] vals = attrc[j].Value.Split(delimiterChars);
					Vector3 scale;
					scale.x = float.Parse(vals[0]);
					scale.y = float.Parse(vals[1]);
					scale.z = float.Parse(vals[2]);
					clone.transform.localScale = scale;
				}
			}
		clone.transform.parent = level.transform;
		}		
    }
}