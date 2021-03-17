﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public CharacterData CharacterData => characterData;
    [Header("Datas")]
    [SerializeField] private CharacterData characterData = null;
    private Dictionary<int, float> coolingDownSkillStartTimeDict;

#region buff
    private List<BuffData> waitingAddBuffList;
    private List<int> waitingRemoveBuffList;
    public Dictionary<int, BuffData> buffIdToBuffDataDict {get; private set;}
#endregion

    [Header("Configs")]
    public Transform weaponIssuePoint; // tmp
    
    [Header("Components")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController characterController;

    void Awake()
    {
        coolingDownSkillStartTimeDict = new Dictionary<int, float>();
#region buff
        waitingAddBuffList = new List<BuffData>();
        waitingRemoveBuffList = new List<int>();
        buffIdToBuffDataDict = new Dictionary<int, BuffData>();
#endregion
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        agent.stoppingDistance = 1;
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
        characterData.CharacterState = CharacterState.Idle;
    }

    public void Move(Vector3 endPos)
    {
        agent.SetDestination(endPos);
        TowardDir(endPos - transform.position);
        characterData.CharacterState = CharacterState.Moving;
    }

    public void TowardDir(Vector3 dir)
    {
        Quaternion rotation = Quaternion.LookRotation(dir);
        float rotationY = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            rotation.eulerAngles.y,
            ref characterData.rotateSmoothTime,
            characterData.rotateSpeedMovement * Time.deltaTime
        );
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    public void IssueSkill(int skillId)
    {
        if (skillId == 0)
        {
            Debug.Log("传入参数不合法 skillId == 0");
            return;
        }
        SkillProjectData skillProjectData = new SkillProjectData(this, characterData.GetSkillData(skillId), Vector3.zero);
        IssueSkill(skillProjectData);
    }

    public void IssueSkill(SkillProjectData skillProjectData)
    {
        var skillData = skillProjectData.skillData;
        if (skillData == null)
        {
            Debug.Log("传入参数 skillData == null");
            return;
        }
        int skillId = skillData.id;
        if (!CanIssueSkill()) {
            Debug.Log("当前状态不能发动技能");
            return;
        }
        if (characterData.GetSkillData(skillId) != skillData)
        {
            Debug.Log(string.Format("找不到对应的skillId={0}的数据", skillId));
            return;
        }
        if (IsSkillCoolDowning(skillId))
        {
            Debug.Log(string.Format("当前skillId={0}未冷却完成", skillId));
            return;
        }
        if (skillData.RangeType != RangeType.None && skillData.flySpeed > 0)
        {
            GameObject prefab = Resources.Load<GameObject>(skillData.resPath);
            if (prefab == null) {
                Debug.LogError("找不到Resources资源 path= " + skillData.resPath);
                return;
            }
            var skillProject = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity).GetComponent<SkillProject>();
            skillProject.Init(skillProjectData);
        }
        skillData.isCoolDowning = true;
        coolingDownSkillStartTimeDict.Add(skillId, Time.realtimeSinceStartup);
        return;
    }

    void Update()
    {
        if (IsDead())
            return;
        
        // 技能冷却中
        if (coolingDownSkillStartTimeDict.Count > 0)
        {   
            List<int> removeList = new List<int>();
            foreach(var item in coolingDownSkillStartTimeDict) {
                var skillData = characterData.GetSkillData(item.Key);
                float endTime = item.Value + skillData.coolDownTime;
                if (Time.realtimeSinceStartup >=  endTime && skillData.isCoolDowning) {
                    skillData.isCoolDowning = false;
                    removeList.Add(item.Key);
                }
            }

            for (int i = 0; i < removeList.Count; i ++) {
                coolingDownSkillStartTimeDict.Remove(removeList[i]);
                Debug.Log("技能冷却完毕skillId:" + removeList[i]);
            }
        }

        // 角色移动
        if(!agent.isStopped) {
            float speed = agent.velocity.magnitude / agent.speed;
            anim.SetFloat("Speed", speed, characterData.moveSpeedDampTime, Time.deltaTime);
        } else if (characterData.CharacterState == CharacterState.Moving) {
            characterData.CharacterState = CharacterState.Idle;
        }

#region buff
        // add buff
        if (waitingAddBuffList.Count > 0)
        {
            foreach(var buff in waitingAddBuffList) {
                int buffId = buff.id;
                BuffData buffData = GetBuffData(buffId);
                if (buffData == null) {
                    buffIdToBuffDataDict.Add(buffId, buff);
                } else {
                    if (buffData.canOverlay)
                        buffData.OverlayBuff(buffData);
                }
            }
            waitingAddBuffList.Clear();
        }

        // remove buff
        if (waitingRemoveBuffList.Count > 0) {
            foreach(int buffId in waitingRemoveBuffList) {
                if (GetBuffData(buffId) != null) {
                    buffIdToBuffDataDict.Remove(buffId);
                }
            }
            waitingRemoveBuffList.Clear();
        }

        // handle buff
        if (buffIdToBuffDataDict.Count > 0) {
            foreach(var item in buffIdToBuffDataDict) {
                int buffId = item.Key;
                var buffData = item.Value;
                if (buffData.IsObsolete()) {
                    TakeOffBuff(buffId);
                    continue;
                }
                if (buffData.CanHandle())
                {
                    buffData.Handle();
                }
            }
        }
#endregion
    }

    public void JoyStick(Vector3 dir)
    {   
        if (CharacterData.CharacterState == CharacterState.Dead) return;
        float speed = agent.speed;
        TowardDir(dir);
        transform.position += dir * speed * Time.deltaTime * characterData.moveSpeedDampTime;
        anim.SetFloat("Speed", speed, characterData.moveSpeedDampTime, Time.deltaTime);
    }

    public bool CanIssueSkill()
    {
        return characterData.CharacterState != CharacterState.Dead;
    }

    public bool IsDead() {
        return characterData.CharacterState == CharacterState.Dead;
    }

    public int GetHP() {
        return characterData.HP;
    }

    public int GetMP() {
        return characterData.MP;
    }

    public float GetCoolDownStartTimeStamp(int skillId)
    {
        if (IsSkillCoolDowning(skillId)) {
            float res;
            if (coolingDownSkillStartTimeDict.TryGetValue(skillId, out res)){
                return res;
            }
            Debug.Log(string.Format("找不到skillId={0}的startTimeStamp", skillId));
        }
        return Time.realtimeSinceStartup;
    }

    public bool IsSkillCoolDowning(int skillId)
    {
        var skillData = characterData.GetSkillData(skillId);
        if (skillData.id == 0) {
            Debug.Log(string.Format("找不到skillId={0}的数据", skillId));
            return false;
        }
        return skillData.isCoolDowning;
    }

    // temp
    public void LearnSkill(SkillData skillData)
    {
        characterData.LearnSkill(skillData);
    }

    public void TakeDamege(int damge)
    {
        if (IsDead()) return;
        Debug.Log("扣血TakeDamege: " + damge);
        characterData.HP = Mathf.Max(0, characterData.HP - damge);
        if (GetHP() == 0) {
            characterData.CharacterState = CharacterState.Dead;
            Debug.Log("<color=#00FF00> ========== dead ============= </color>");
        }
    }

    public void PutOnBuff(BuffData buffData)
    {
        if (buffData == null || IsDead()) return;
        waitingAddBuffList.Add(buffData);
        Debug.Log(string.Format("PutOnBuff: [{0}]", buffData.name));
    }

    public void TakeOffBuff(int buffId)
    {
        BuffData buffData = GetBuffData(buffId);
        if (buffData != null) {
            if (buffData.isEnable) {
                waitingRemoveBuffList.Add(buffId);
            }
            buffData.KillSelf();
        }
    }

    public void CleanBuff()
    {
        buffIdToBuffDataDict.Clear();
        waitingAddBuffList.Clear();
        waitingRemoveBuffList.Clear();
    }

    public BuffData GetBuffData(int buffId) {
        BuffData buffData;
        if (buffIdToBuffDataDict.TryGetValue(buffId, out buffData)) {
            return buffData;
        }
        return null;
    }
}
