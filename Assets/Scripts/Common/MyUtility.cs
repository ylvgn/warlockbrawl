using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtility
{
    public static T GetComponent<T>(Transform transform, string path = "")
    {
        T res;
        if (path != "") {
            var obj = transform.Find(path);
            if (obj == null) {
                Debug.LogError(" GetComponent 找不到该路径 path = " + path);
                return default(T);
            }
            res = obj.GetComponent<T>();
        } else res = transform.GetComponent<T>();
        if (res == null) {
            Debug.LogError("找不到component, path=" + path);
        }
        return res;
    }

    public static T GetComponentInChildren<T>(Transform transform)
    {
        var res = transform.GetComponentInChildren<T>();
        if (res == null) {
            Debug.LogError("找不到 GetComponentInChildren " + typeof(T).ToString());
        }
        return res;
    }
    
    public static T GetComponentInParent<T>(Transform transform)
    {
        var res = transform.GetComponentInParent<T>();
        if (res == null) {
            Debug.LogError("找不到 GetComponentInParent " + typeof(T).ToString());
        }
        return res;
    }
    
    public static void MyDebug(string format, params object[] args)
    {
        Debug.Log("<color=#00FF00> MyDebug: </color>" + string.Format(format, args));
    }
}
