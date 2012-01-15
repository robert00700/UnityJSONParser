using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSON {
	//Holder for value
	private JSONValue val;
	
	public JSON() {
		
	}
	
	public JSON(JSONValue val) {
		this.val = val;
	}
	
	//Getter/Setter for Boolean
	public bool boolean() {
		if(val is JSONBoolean) {
			return ((JSONBoolean)val).val;
		}
		else return false;
	}
	public void boolean(bool v) {
		val = new JSONBoolean();
		((JSONBoolean)val).val = v;
	}
	
	//Getter/Setter for Number
	public double num() {
		if(val is JSONNumber) {
			return ((JSONNumber)val).val;
		}
		else return 0.0;
	}
	public void num(double v) {
		val = new JSONNumber();
		((JSONNumber)val).val = v;
	}
	
	//Getter/Setter for String
	public string str() {
		if(val is JSONString) {
			return ((JSONString)val).val;
		}
		else return "";
	}
	public void str(string v) {
		val = new JSONString();
		((JSONString)val).val = v;
	}
	
	//Getter/Setter for Vector 
	public Vector3 vec3() {
		if(val is JSONObject) {
			return new Vector3((float)this["x"].num(), (float)this["y"].num(), (float)this["z"].num());
		}
		else return Vector3.zero;
	}
	public void vec3(Vector3 v) {
		val = new JSONObject();
		this["x"].num (v.x);
		this["y"].num (v.y);
		this["z"].num (v.z);
	}
	
	//Getter/Setter for Object
	public JSON this[string key] {
		get {
			if(val is JSONObject) {
				JSONObject obj = (JSONObject)val;
				if(obj.elements.ContainsKey(key)) {
					return obj.elements[key];
				}
				else {
					JSON newObj = new JSON(new JSONObject());
					obj.elements.Add(key, newObj);
					return newObj;
				}
			}
			else {
				val = new JSONObject();
				JSONObject obj = (JSONObject)val;
				JSON newObj = new JSON(new JSONObject());
				obj.elements.Add(key, newObj);
				return newObj;
			}
		}
		set {}
	}
	
	//Getter/Setter for Array
	public JSON this[int key] {
		get {
			if(val is JSONArray) {
				JSONArray obj = (JSONArray)val;
				if(obj.elements.Count > key) {
					return obj.elements[key];
				}
				else {
					JSON newObj = new JSON(new JSONArray());
					obj.elements[key] = newObj;
					return newObj;
				}
			}
			else {
				val = new JSONArray();
				JSONArray obj = (JSONArray)val;
				JSON newObj = new JSON(new JSONArray());
				obj.elements[key] = newObj;
				return newObj;
			}
		}
		set {}
	}
	
	//Appender for Array
	public JSON Append(JSON add) {
		//Convert to JSONArray if not
		if(!(val is JSONArray)) {
			val = new JSONArray();
		}
		
		JSONArray arr = (JSONArray)val;
		arr.elements.Add(add);
		return this;
	}
	
	//Iterator for array
	public System.Collections.IEnumerator GetEnumerator() {
		//Array iterator
		if(val is JSONArray) {
			JSONArray arr = (JSONArray)val;
	    	for (int i = 0; i < arr.elements.Count; i++)
	    	{
	       	 	yield return arr.elements[i];
	    	}
		}
		//Object iterator
		/*else if(val is JSONObject) {
			JSONObject obj = (JSONObject)val;
			foreach (KeyValuePair<string, JSON> pair in obj) {
				yield return pair;
			}
			}*/
	}
	
	public string Stringify(string indent) {
		return val.Stringify(indent);
	}
};

	public abstract class JSONValue {	
		public abstract string Stringify(string indent);
	};
	
	public class JSONObject : JSONValue {
		public Dictionary<string, JSON> elements;
		public JSONObject() {
			elements = new Dictionary<string, JSON>();
		}
		public override string Stringify (string indent) {
			string ret = "{\n";
			foreach (KeyValuePair<string, JSON> pair in elements) {
				ret += indent + '"' + pair.Key + '"' + " : " + pair.Value.Stringify(indent + "\t") + ",\n";
			}
			ret = ret.Remove(ret.Length - 2, 2);
			ret += "\n" + indent + "}";
			return ret;
		}
	};
	
	public class JSONObjectPair {
		public JSONString key;
		public JSONValue val;
	};
	
	public class JSONArray: JSONValue {
	public int current = 0;
	public List<JSON> elements;
		public JSONArray() {
			elements = new List<JSON>();
		}
		public override string Stringify (string indent) {
			string ret = "\n"+ indent +"[\n";
			foreach (JSON j in elements) {
				ret += indent + j.Stringify(indent + "\t") + ",\n";
			}
			ret = ret.Remove(ret.Length - 2, 2);
			ret += "\n" + indent + "]";
			return ret;
		}
	};
	
	public abstract class JSONPrimitive: JSONValue {
	};
	
	public class JSONString : JSONPrimitive {
		public string val;
		public override string Stringify (string indent) {
			return '"' + val + '"';
		}
	};
	
	public class JSONNumber : JSONPrimitive {
		public double val;
		public override string Stringify (string indent) {
			return val.ToString();
		}
	};
	
	public class JSONBoolean : JSONPrimitive {
		public bool val;
		public override string Stringify (string indent) {
			return val.ToString();
		}
	};

	public class JSONNull : JSONPrimitive {
		public override string Stringify (string indent) {
			return "null";	
		}
	}


public class JSONParser {
	public delegate void ParseURLResponse(JSON parse);
	
	private void db(string db) {
		//Debug.Log(db + "::::::" +  Random.value);
	}
	
	private string data;
	private int ptr;
	
	private char tok() {
		return (ptr > (data.Length - 1)) ? 'x' : data[ptr];
	}
	
	private void lex() {
		ptr++;
	}
	
	private void parseWhitespace() {
		while(tok() == ' ' || tok() == '\n' || tok() == '\t') lex();
	}
	
	private void parseChar(char i, bool prewhite, bool postwhite) {
		//Get rid of whitespace if we are not trying to parse whitespace!
		if(i != ' ' && i != '\n' && i != '\t' && prewhite) {
			parseWhitespace();
		}
		if(tok() == i) lex();
		else {
			//Error
		}
		if(i != ' ' && i != '\n' && i != '\t' && postwhite) {
			parseWhitespace();
		}
	}
	
	private JSONArray ParseArray() {
		db("parsing array");
		JSONArray ret = new JSONArray();
		parseChar('[', true, true);
		while(tok() != ']') {
			ret.elements.Add(new JSON(ParseValue()));
			parseChar(',', true, true); //todo: error check
		}
		parseChar(']', true, true);
		return ret;
	}
	
	private JSONObject ParseObject() {
		db("parsing object");
		JSONObject ret = new JSONObject();
		parseChar('{', true, true);
		while(tok() != '}') {
			JSONObjectPair pair = ParseObjectPair();
			ret.elements.Add(pair.key.val, new JSON(pair.val));
			parseChar(',', true, true); //todo: error check
		}
		parseChar('}', true, true);
		return ret;
	}
	
	private JSONObjectPair ParseObjectPair() {
		db("parsing pair");
		JSONObjectPair ret = new JSONObjectPair();
		ret.key = ParseString();
		parseChar(':', true, true);
		ret.val = ParseValue();
		return ret;
	}
	
	private JSONString ParseString() {
		db("parsing string");
		JSONString ret = new JSONString();
		parseChar('"', true, false);
		bool slashed = false;
		int start = ptr;
		int length = 0;
		while(!(tok() == '"' && !slashed)) {
			if(tok() == '\\' && !slashed) slashed = true;
			else slashed = false;
			lex();
			length++;
		}
		ret.val = data.Substring(start, length);
		parseChar('"', false, true);
		return ret;
	}
	
	private JSONNumber ParseNumber() {
		db("parsing number");
		JSONNumber ret = new JSONNumber();
		int start = ptr;
		int length = 0;
		while((tok() >= '0' && tok() <= '9') || tok() == '.' || tok() == '-' || tok() == '+' || tok() == 'e' || tok() == 'E') {
			lex();
			length++;
		}
		db("attempting to parse " + data.Substring(start, length));
		ret.val = double.Parse(data.Substring(start, length));
		return ret;
	}
	
	private JSONBoolean ParseBoolean() {
		JSONBoolean ret = new JSONBoolean();
		if(data.Substring(ptr, 4) == "true") {
			ret.val = true;
			ptr += 4;
		}
		else if(data.Substring(ptr, 5) == "false") {
			ret.val = false;
			ptr += 5;
		}
		return ret;
	}
	
	private JSONNull ParseNull() {
		if(data.Substring(ptr, 4) == "null") {
			ptr += 4;
			return new JSONNull();
		}
		else return null;
	}
	
	private JSONValue ParseValue() {
		db("parsing value");
		JSONValue ret;
		switch(tok()) {
		case '{':
				ret = ParseObject();
				break;
		case '[':
				ret = ParseArray();
				break;
		case '"':
				ret = ParseString();
				break;
		case 't':
		case 'f':
				ret = ParseBoolean();
				break;
		case 'n':
				ret = ParseNull();
				break;
		default:
				ret = ParseNumber();
				break;	
		}
		return ret;
	}
	
	private JSONParser(string data) {
		this.data = data;
		ptr = 0;
	}
	
	public static JSON ParseString(string data) {
		JSONParser parse = new JSONParser(data);
		return new JSON(parse.ParseValue());
	}
	
	public static IEnumerator ParseURL(string url, ParseURLResponse callback) {
		WWW req = new WWW(url);
		yield return req;
		callback(ParseString(req.text));
	}
}
