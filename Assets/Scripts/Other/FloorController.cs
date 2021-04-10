using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private bool isEnable;
    private int timerId;
    private float maxRadius;
    public float curRadius { get; private set;}
    Dictionary<Character, float> characterAddBuffStartTimeDict;

    [Header("Config")]
    public Transform floorBorder;
    public float floorShrinkDelayTime = 3f;
    public float floorShrinkIntervalTime = 2f;
    public float intervalAddBuffTime = 4f;

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
        if (StatsManager.Instance.CharacterList == null) return;

        var characterList = StatsManager.Instance.CharacterList;
        if (characterList.Count > 0)
        {
            foreach(var character in characterList) {
                float dist = Vector3.Distance(Vector3.zero, character.transform.position);
                var characterData = character.CharacterData;
                var buffData = characterData.GetBuffData(BuffType.DropHP);
                Debug.DrawLine(Vector3.zero, character.transform.position, Color.green);
                if (dist >= curRadius) // 站岩浆越久，扣血越多
                {
                    float startTime;
                    if (characterAddBuffStartTimeDict.TryGetValue(character, out startTime))
                    {
                        if (startTime + intervalAddBuffTime < Time.realtimeSinceStartup) {
                            character.PutOnBuff(MakeBuffData(character));
                            characterAddBuffStartTimeDict[character] = Time.realtimeSinceStartup;
                        }
                    } else {
                        characterAddBuffStartTimeDict.Add(character, Time.realtimeSinceStartup);
                        character.PutOnBuff(MakeBuffData(character));
                    }
                } else if (buffData != null) { // 回到地面
                    characterAddBuffStartTimeDict.Remove(character);
                    characterData.TakeOffBuff(BuffType.DropHP);
                }
            }
        }
    }

    public void ReStart()
    {
        isEnable = true;
        characterAddBuffStartTimeDict.Clear();
        TimerManager.Instance.RemoveTimer(timerId);
        curRadius = maxRadius;
        float testFloat = 0;
        int level = 1;
        timerId = TimerManager.Instance.AddLoop(floorShrinkDelayTime, floorShrinkIntervalTime, () => {
            curRadius = maxRadius / level;
            testFloat += 0.1f;
            level ++;
            Shader.SetGlobalFloat("testFloat", testFloat);
            //Debug.Log(string.Format("缩圈: testFloat={0}, timerId={1}, curRadius={2}", testFloat, timerId, curRadius));
            if (testFloat >= 1) {
                curRadius = maxRadius / 9;
                TimerManager.Instance.RemoveTimer(timerId);
            }
        });
    }

    DropHPBuffData MakeBuffData(Character owner) // tmp
    {
        return new DropHPBuffData(owner, 100, 3, 5);
    }
}
