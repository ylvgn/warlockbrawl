using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [Header("Datas")][SerializeField]

    private CharacterData characterData;
    private int curSkillId = -1;

    [Header("Components")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController characterController;


    public static event Action<CharacterData, int> OnFireSkill;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    public void SetData(CharacterData data)
    {
        this.characterData = data;
    }

    public void Move(Vector3 dest)
    {
        var data = this.characterData;
        agent.SetDestination(dest);
        Quaternion rotation = Quaternion.LookRotation(dest - transform.position);

        float rotate_y = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            rotation.eulerAngles.y,
            ref data.roleData.rotateSmoothTime,
            data.roleData.rotateSpeedMovement * Time.deltaTime * 5
        );
        transform.rotation = Quaternion.Euler(0, rotate_y, 0);
        StartCoroutine(MoveAnim());
    }

    IEnumerator MoveAnim()
    {
        var data = this.characterData;
        while (!agent.isStopped)
        {
            float speed = agent.velocity.magnitude / agent.speed;
            anim.SetFloat("Speed", speed, data.roleData.moveSpeedDampTime, Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void Attack(int skillId)
    {
        var skillData = this.characterData.GetSkillData(skillId);
        if (skillData == null)
        {
            Debug.Log(string.Format("不存在该skillID={0}", skillId));
            return;
        }
        skillData.isCoolDowning = true;
        this.CancleSelectSkill();
        if (OnFireSkill != null) OnFireSkill(this.characterData, skillId);
    }

    public void SelectSkill(int skillId)
    {
        if (curSkillId != -1)
        {
            Debug.Log(string.Format("已选择skillID{0}, 不可同时选择 skillDID{1}", curSkillId, skillId));
            return;
        }
        curSkillId = skillId;

    }

    public void CoolDownFinish(int skillId)
    {
        var skillData = this.characterData.GetSkillData(skillId);
        if (skillData == null)
        {
            Debug.Log(string.Format("skillID={0} 不存在", skillId));
            return;
        }
        
        skillData.isCoolDowning = false;
    }

    public void CancleSelectSkill()
    {
        curSkillId = -1;

    }

    private void OnDestroy()
    {
        OnFireSkill = null;
    }
}
