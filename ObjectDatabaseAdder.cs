using UnityEngine;
using System.Collections;

public class ObjectDatabaseAdder : MonoBehaviour
{
	public GameObject[] objects;
	void Start ()
	{
		foreach (GameObject obj in objects) {
			ObjectDatabaseManager.addObject(obj.name, obj);	
		}
	}
}

