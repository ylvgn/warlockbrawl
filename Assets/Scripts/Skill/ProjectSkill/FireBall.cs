using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : SkillProject
{   
    private bool isEnable;

    public override void Init(SkillProjectData data) {
        base.Init(data);
        if (Owner.IsDead()) {
            return;
        }

        Vector3 dir = getDir();
        float offsetY = Owner.characterController.height;
        var startPos = Owner.transform.position + new Vector3(0, offsetY / 2, 0) + dir;
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
        if (Vector3.Distance(transform.position, StartPos) > skillData.maxRange) {
            isEnable = false;
            GameObject.Destroy(gameObject);
        }
    }
    
    public override void OnCollisionEnter(Collision collision) {
        if (!isEnable) return;
        Character enemy = collision.gameObject.GetComponent<Character>();
        if (enemy == Owner) return;
        if (enemy && !enemy.IsDead()) {
            var damge = GetDamage(enemy);
            enemy.TakeDamage(damge.CalcDamage());
        }
        GameObject.Destroy(gameObject);
    }

    public override DamgeData GetDamage(IAttackable other)
    {
        var skillData = getSkillData();
        var enemy = other as Character;
        return new DamgeData(Owner.CharacterData, skillData, enemy.CharacterData);
    }
}
