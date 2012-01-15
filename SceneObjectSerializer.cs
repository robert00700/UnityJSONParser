using UnityEngine;
using System.Collections;

public class SceneObjectSerializer {
	public static void DeserializeSceneObjects(JSON objects) {
		foreach (JSON j in objects) {
			string name = j["name"].str();
			Vector3 pos =  j["position"].vec3();
			GameObject go = (GameObject)GameObject.Instantiate(ObjectDatabaseManager.getObject(name), pos, Quaternion.identity);
			go.name = name;
		}
	}
	
	public static JSON SerializeSceneObjects(bool die) {
		GameObject[] save = GameObject.FindGameObjectsWithTag("Editor");
		JSON objects = new JSON();
		foreach (GameObject go in save) {
			JSON obj = new JSON();
			obj["name"].str(go.name);
			obj["position"].vec3(go.transform.position);
			objects.Append(obj);
			if(die) {
				GameObject.Destroy(go);
			}
		}
		return objects;
	}
}

