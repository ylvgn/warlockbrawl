using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloorController : MonoBehaviour
{
    bool isEnable;
    private int timerId;
    private float maxRadius;
    public float curRadius { get; private set;}

    [Header("Config")]
    public float floorShrinkDelayTime = 3f;
    public float floorShrinkIntervalTime = 2f;
    public Transform floorBorder;
    public AnimationCurve floorShrinkCurve;

    public float intervalAddBuffTime = 4f;
    Dictionary<Character, float> characterAddBuffStartTimeDict;
    private int DropHPBuffID = 1; // tmp

    void Awake()
    {
        characterAddBuffStartTimeDict = new Dictionary<Character, float>();
    }

    public void Init()
    {
        maxRadius = Vector3.Distance(Vector3.zero, floorBorder.position);
        Shader.SetGlobalFloat("testFloat", 0.1f);
        isEnable = false;
    }

    void Update()
    {
        if (!isEnable || StatsManager.Instance == null) return;
        if (StatsManager.Instance.characterList != null)
        {
            var characterList = StatsManager.Instance.characterList;
            if (characterList.Count > 0) {
                foreach(var character in characterList) {
                    float dist = Vector3.Distance(Vector3.zero, character.transform.position);
                    Debug.DrawLine(Vector3.zero, character.transform.position, Color.green);
                    if (dist >= curRadius) // 站岩浆越久，扣血越多
                    {
                        float startTime;
                        if (characterAddBuffStartTimeDict.TryGetValue(character, out startTime))
                        {
                            if (startTime + intervalAddBuffTime < Time.realtimeSinceStartup) {
                                BuffData buff = ResManager.Instance.CreateBuff(DropHPBuffID, character);
                                character.PutOnBuff(buff);
                                characterAddBuffStartTimeDict[character] = Time.realtimeSinceStartup;
                            }
                        } else {
                            characterAddBuffStartTimeDict.Add(character, Time.realtimeSinceStartup);
                            BuffData buff = ResManager.Instance.CreateBuff(DropHPBuffID, character);
                            character.PutOnBuff(buff);
                        }
                    } else if (character.GetBuffData(DropHPBuffID) != null) { // 回到地面
                        characterAddBuffStartTimeDict.Remove(character);
                        character.TakeOffBuff(DropHPBuffID);
                    }
                }
            }
        }
    }

    public void ReStart()
    {
        isEnable = true;
        TimerManager.Instance.RemoveTimer(timerId);
        curRadius = maxRadius;
        float testFloat = 0;
        int level = 1;
        timerId = TimerManager.Instance.AddLoop(floorShrinkDelayTime, floorShrinkIntervalTime, () => {
            curRadius = maxRadius / level;
            testFloat += 0.1f;
            level ++;
            Shader.SetGlobalFloat("testFloat", testFloat);
            Debug.Log(string.Format("缩圈: testFloat={0}, timerId={1}, curRadius={2}", testFloat, timerId, curRadius));
            if (testFloat >= 1) {
                curRadius = maxRadius / 9;
                TimerManager.Instance.RemoveTimer(timerId);
            }
        });
    }

    void OnDestroy()
    {
        Shader.SetGlobalFloat("testFloat", 0.1f);
    }
}
