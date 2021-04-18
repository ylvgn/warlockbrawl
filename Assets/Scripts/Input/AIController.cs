using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Character character;
    public float _DetectEnemyRadius = 100f; // 探测敌人半径

    void Start()
    {
        character = MyUtility.GetComponent<Character>(transform);
        var healthData = new HealthData(character.CharacterData.health.HP, character.CharacterData.health.MP);
        ResManager.Instance.BuildHUD(character, healthData);
    }

    public void Walk(Vector3 pos)
    {
        character.Move(pos);
    }

    public Character FindNearestEnemy()
    {
        Character res = null;
        var characterList = StatsManager.Instance.CharacterList;
        float nearestDistance = _DetectEnemyRadius * _DetectEnemyRadius;
        foreach (var other in characterList)
        {
            if (other.IsDead() || other == character) continue;
            float distance2 = (character.transform.position - other.transform.position).sqrMagnitude;
            if (nearestDistance > distance2 && distance2 > 0)
            {
                nearestDistance = distance2;
                res = other;
            }
        }
        return res;
    }

    public void Attack(Character enemy)
    {
        if (enemy == null || enemy.IsDead()) return;
        if (character.CanIssueSkill())
            StartCoroutine(WalkCloseAndAttack(enemy));
    }

    IEnumerator WalkCloseAndAttack(Character enemy)
    {
        var skillData = character.CharacterData.GetSkillData(1); // tmp
        float range = skillData.maxRange;
        float distance = Vector3.Distance(enemy.transform.position, character.transform.position);

        character.anim.SetBool("FindEnemy", true);
        while (!enemy.IsDead() && distance > range && distance < _DetectEnemyRadius && character.CanIssueSkill())
        {
            character.anim.SetBool("CanWalk", true);
            distance = Vector3.Distance(enemy.transform.position, character.transform.position);
            Walk(enemy.transform.position);
            yield return new WaitForSeconds(0.02f);
        }

        character.anim.SetBool("FindEnemy", false);
        if (!enemy.IsDead() && distance <= range)
        {
            SkillProjectData skillProjectData = new SkillProjectData(character, skillData, enemy.transform.position);
            Attack(skillProjectData);
        }
    }

    public void Attack(SkillProjectData skillProjectData)
    {
        character.IssueSkill(skillProjectData);
    }
}
