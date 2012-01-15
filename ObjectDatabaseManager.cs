using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ObjectDatabaseManager
{
	public static Dictionary<string, GameObject> database = new Dictionary<string, GameObject>();
	public static GameObject getObject(string name) {
		if(database.ContainsKey(name)) {
			return database[name];
		}
		else return null;
	}
	public static void addObject(string name, GameObject obj) {
		database.Add(name, obj);
	}
}

