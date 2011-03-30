// PlayUp Tools - www.playuptools.com

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;

using System;
using System.Xml;

public class PlayUpRC {
	
	[MenuItem ("Assets/Import Level via PlayUp")]
	
	static void  Init () {
		UnityEngine.Object obj = Selection.activeObject;
		string path = AssetDatabase.GetAssetPath(obj);
		string fn = Path.GetFileName(path);
		if ((Path.GetExtension(fn) == ".lvl") && (Path.GetDirectoryName(path) == "Assets/PlayUp/Levels")) {
			FiletoObj(fn);
		}
		else {
			if (Path.GetExtension(fn) != ".lvl")	Debug.Log("The file you have selected is not a valid .LVL file.");
			if (Path.GetDirectoryName(path) != "Assets/PlayUp/Levels") Debug.Log("The file you have selected is not located in the Assets/PlayUp/Levels directory.");
		}
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
					Vector3 rotAA;
					float axisangle;
					rotAA.x = float.Parse(vals[0]);
					rotAA.z = float.Parse(vals[1]);
					rotAA.y = float.Parse(vals[2]);
					axisangle = -float.Parse(vals[3]);		
					clone.transform.rotation = Quaternion.AngleAxis(axisangle, rotAA);
					clone.transform.Rotate(Vector3.right * 90);
					clone.transform.Rotate(Vector3.up * -180);
				} 
				if (attrc[j].Name == "SCALE"){
					string[] vals = attrc[j].Value.Split(delimiterChars);
					Vector3 scale;
					scale.x = float.Parse(vals[0]);
					scale.y = float.Parse(vals[1]);
					scale.z = float.Parse(vals[2]);
					clone.transform.localScale = scale;
					if ((scale.x > 0) && (scale.y < 0) && (scale.z < 0)){
						clone.transform.Rotate(Vector3.right * 180);
					}
				}
			}
		clone.transform.parent = level.transform;
		}		
    }
}