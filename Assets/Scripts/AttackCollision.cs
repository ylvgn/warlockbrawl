using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    Action<Collision> onCollisionEnterCallBack;
    Action updateHandler;
    bool isCollision;

    public void Init(Action updateHandler, Action<Collision> callback = null)
    {
        this.onCollisionEnterCallBack = callback;
        this.updateHandler = updateHandler;
        Invoke("DeleteSelf", 5); // temp
    }

    void Update()
    {
        if (!isCollision)
        {
            updateHandler();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isCollision = true;
        if (onCollisionEnterCallBack != null)
            onCollisionEnterCallBack(collision);
        this.gameObject.SetActive(false);
    }

    void DeleteSelf()
    {
        GameObject.Destroy(this.gameObject);
    }
}
