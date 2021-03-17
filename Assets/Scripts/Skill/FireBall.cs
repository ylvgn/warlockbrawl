using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : SkillProject
{   
    private bool isEnable;

    public override void Init(SkillProjectData data) {
        base.Init(data);
        var owner = getOwner();
        if (owner == null || owner.IsDead()) {
            return;
        }

        Vector3 dir = getDir();
        float offsetY = owner.characterController.height;
        var startPos = owner.transform.position + new Vector3(0, offsetY / 2, 0) + dir;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(dir);
        GameObject.Destroy(gameObject, 5);
        isEnable = true;
    }

    void Update()
    {
        if (!isEnable) return;
        var skillData = getSkillData();
        transform.Translate(Vector3.forward * skillData.flySpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, StartPos) > 100) {
            isEnable = false;
            GameObject.Destroy(gameObject);
            Debug.Log("超出范围啦");
        }
    }
    
    public override void OnCollisionEnter(Collision collision) {
        if (!isEnable) return;
        Debug.Log("碰撞了！" + collision.gameObject.name);
        Character enemy = collision.gameObject.GetComponent<Character>();
        if (enemy && !enemy.IsDead()) {
            var skillData = getSkillData();
            DamgeData damge = new DamgeData(OwnerData, skillData, enemy.CharacterData);
            enemy.TakeDamege(damge.CalcDamage());
        }

        GameObject.Destroy(gameObject);
    }
}
