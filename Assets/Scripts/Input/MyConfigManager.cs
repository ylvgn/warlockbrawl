using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyConfigManager : MonoBehaviour
{
    private bool isEnable = false;
    public FloorController floorController;

    void Start()
    {
        isEnable = true;
        Selection.activeGameObject = gameObject;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F1))
        {
            isEnable = !isEnable;
            if (isEnable) Selection.activeGameObject = gameObject;
        }
    }

    void OnGUI()
    {
        if (!isEnable) return;
        if (Selection.activeGameObject == gameObject)
        {
            GUI.BeginGroup (new Rect (Screen.width - 100, 0, 100, 120));
            float height = 20;
            float posY = 0;
            if (GUI.Button(new Rect(0, posY, 100, height), "FloorController"))
            {
                Selection.activeGameObject = floorController.gameObject;
            }
            posY += 20;
            if (GUI.Button(new Rect(0, posY, 100, height), "FloorController"))
            {
                Selection.activeGameObject = floorController.gameObject;
            }
            posY += 20;
            if (GUI.Button(new Rect(0, posY, 100, height), "FloorController"))
            {
                Selection.activeGameObject = floorController.gameObject;
            }
            posY += 20;
            if (GUI.Button(new Rect(0, posY, 100, height), "FloorController"))
            {
                Selection.activeGameObject = floorController.gameObject;
            }
            posY += 20;
            if (GUI.Button(new Rect(0, posY, 100, height), "FloorController"))
            {
                Selection.activeGameObject = floorController.gameObject;
            }
            posY += 20;
            if (GUI.Button(new Rect(0, posY, 100, height), "FloorController"))
            {
                Selection.activeGameObject = floorController.gameObject;
            }
            GUI.EndGroup ();
        } else {
            if (GUI.Button(new Rect(0, 0, 100, 100), "Back"))
            {
                Selection.activeGameObject = gameObject;
            }
        }
    }
}
