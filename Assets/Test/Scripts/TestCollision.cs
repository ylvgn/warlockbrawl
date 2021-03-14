using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    public class EffectInfo
    {
        public CharacterData.SkillData skillData;
        public int id;
        public Vector3 start;
        public Vector3 dir;
        public float speed;
    }

    EffectInfo data;
    GameObject obj;
    bool isCollision;
    Action onCollisionEnterCallBack;

    public void Init(EffectInfo data, GameObject obj, Action callback)
    {
        this.obj = obj;
        this.data = data;
        onCollisionEnterCallBack = callback;
        transform.position = data.start;
        //float distance = data.skillData.maxAttackRadius;
        this.obj.transform.LookAt(data.dir);
        Invoke("DeleteSelf", 5);
    }

    private void Update()
    {
        if (data != null && !isCollision)
        {
            //transform.position = Vector3.Lerp(transform.position, data.dir * 1.2f, data.speed * Time.deltaTime); // ???
            transform.Translate(Vector3.forward * data.speed * 10 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isCollision = true;
        Debug.Log(string.Format("碰撞啦:{0}", collision.gameObject.name));
        onCollisionEnterCallBack();
        obj.SetActive(false);
    }

    void DeleteSelf()
    {
        GameObject.Destroy(obj);
    }
}
