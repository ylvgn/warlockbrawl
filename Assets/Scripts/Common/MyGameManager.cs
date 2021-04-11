using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    public FloorController floorController;

    void Start()
    {
        floorController.Init();
        ResetStart();
    }

    public void ResetStart()
    {
        floorController.ReStart();
    }
}
