using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    private bool isEnable;
    private int timerId;
    private float maxRadius;
    public float curRadius { get; private set; }
    Dictionary<Character, float> characterAddBuffStartTimeDict;

    [Header("Config")]
    public Transform floorBorder;
    public float floorShrinkDelayTime = 3f;
    public float floorShrinkIntervalTime = 2f;
    public float intervalAddBuffTime = 4f;
    public AnimationCurve animationCurve;
    public Vector3 center = Vector2.zero;

    void Awake()
    {
        characterAddBuffStartTimeDict = new Dictionary<Character, float>();
        transform.position = center;
    }

    public void Init()
    {
        maxRadius = Vector3.Distance(Vector3.zero, floorBorder.position);
        isEnable = false;
    }

    void Update()
    {
        if (!isEnable || StatsManager.Instance == null) return;
        if (StatsManager.Instance.CharacterList == null) return;

        var characterList = StatsManager.Instance.CharacterList;
        if (characterList.Count <= 0) return;
        var v2 = curRadius * curRadius;
        foreach (var character in characterList)
        {
            float dist2 = (center - character.transform.position).sqrMagnitude;
            var characterData = character.CharacterData;
            var buffData = characterData.GetBuffData(BuffType.DropHP);
            Debug.DrawLine(Vector3.zero, character.transform.position, Color.green);
            if (dist2 >= v2) // 站岩浆越久，扣血越多
            {
                float startTime;
                if (characterAddBuffStartTimeDict.TryGetValue(character, out startTime))
                {
                    if (startTime + intervalAddBuffTime > Time.realtimeSinceStartup) continue;
                    var addBuffData = ResManager.Instance.BuildBuffData<DropHPBuffData>(BuffType.DropHP, character);
                    character.PutOnBuff(addBuffData);
                    characterAddBuffStartTimeDict[character] = Time.realtimeSinceStartup;
                }
                else
                {
                    var addBuffData = ResManager.Instance.BuildBuffData<DropHPBuffData>(BuffType.DropHP, character);
                    characterAddBuffStartTimeDict.Add(character, Time.realtimeSinceStartup);
                    character.PutOnBuff(addBuffData);
                }
            }
            else if (buffData != null) // 回到地面
            {
                if (characterAddBuffStartTimeDict.Remove(character))
                    characterData.TakeOffBuff(BuffType.DropHP);
            }
        }
    }

    public void ReStart()
    {
        isEnable = true;
        characterAddBuffStartTimeDict.Clear();
        TimerManager.Instance.RemoveTimer(timerId);
        curRadius = maxRadius;
        float testFloat = 0.1f;
        float stepFloat = 0.1f;
        Shader.SetGlobalFloat("testFloat", stepFloat);
        float m = (9 - 1) / (1 - 0.1f);
        timerId = TimerManager.Instance.AddLoop(floorShrinkDelayTime, floorShrinkIntervalTime, () =>
        {
            if (testFloat >= 1)
            {
                TimerManager.Instance.RemoveTimer(timerId);
            } else
            {
                float x = animationCurve.Evaluate(testFloat);
                float y = m * (x - 0.1f) + 1;
                curRadius = maxRadius / y; // shader[0.1, 1] <-> floor[1, 9]
                testFloat += stepFloat;
                Shader.SetGlobalFloat("testFloat", x);
                //Debug.Log($"缩圈: f({testFloat})={x}, timerId={timerId}, fenmu={y}, curRadius={curRadius}");
            }
        });
    }

    void OnDrawGizmos()
    {
        if (!isEnable) return;
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, curRadius);
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject) return;
        float groupWidth = 200f;
        float posY = 0, height = 20, spacingY = 30;
        GUI.BeginGroup(new Rect(Screen.width - groupWidth, 0, groupWidth, Screen.height));
        GUI.Box(new Rect(0, 0, groupWidth, Screen.height), "Floor");
        posY += spacingY;
        GUI.Label(new Rect(0, posY, groupWidth, height), $"缩圈延迟 : {floorShrinkDelayTime}s");
        posY += spacingY;
        floorShrinkDelayTime = GUI.HorizontalSlider(new Rect(0, posY, groupWidth, height), floorShrinkDelayTime, 0, 10);
        posY += spacingY;
        GUI.Label(new Rect(0, posY, groupWidth, height), $"刷新间隔时间 : {floorShrinkIntervalTime}s");
        posY += spacingY;
        floorShrinkIntervalTime = GUI.HorizontalSlider(new Rect(0, posY, groupWidth, height), floorShrinkIntervalTime, 0.1f, 10f);
        posY += spacingY;
        GUI.Label(new Rect(0, posY, groupWidth, height), $"掉血叠加buff间隔时间 : {intervalAddBuffTime}s");
        posY += spacingY;
        intervalAddBuffTime = GUI.HorizontalSlider(new Rect(0, posY, groupWidth, height), intervalAddBuffTime, 0.1f, 10f);
        GUI.EndGroup();
    }
#endif
}
