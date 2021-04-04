using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillIndicator : MonoBehaviour
{
    Dictionary<int, Sprite> spriteAssetsDict;
    Image indicatorImg;
    Image indicatorRangeImg;

    public bool IsEnable => isEnable;
    bool isEnable;
    public SkillData curSkillData;

    void Awake()
    {
        spriteAssetsDict = new Dictionary<int, Sprite>();
        indicatorImg = MyUtility.GetComponent<Image>(transform, "indicator");
        indicatorRangeImg = MyUtility.GetComponent<Image>(transform, "indicatorRange");
        CancleSkill();
    }

    public void SetData(SkillData skillData)
    {
        curSkillData = skillData;
        Sprite sprite = null;
        var RangeType = skillData.RangeType;
        spriteAssetsDict.TryGetValue((int)RangeType, out sprite);
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>(string.Format("Textures/indicator_{0}", (int)RangeType)); // tmp
            if (sprite == null)
            {
                Debug.LogError(string.Format("找不到该rangeType={0}的sprite图片", RangeType));
                return;
            }
            spriteAssetsDict[(int)RangeType] = sprite;
        }

        indicatorImg.sprite = sprite;
        indicatorImg.gameObject.SetActive(true);

        float range = skillData.maxRange;
        float radius = skillData.maxRadius;

        switch (RangeType)
        {
            case RangeType.None:
                break;
            case RangeType.Point:
                indicatorImg.transform.localScale = new Vector3(radius, radius, 1);
                break;
            case RangeType.ArcShaped:
                break;
            case RangeType.Circle:
                indicatorImg.transform.localScale = new Vector3(radius, radius, 1);
                indicatorRangeImg.transform.localScale = new Vector3(range, range, 1);
                indicatorRangeImg.gameObject.SetActive(true);
                break;
            case RangeType.DirectLine:
                indicatorImg.transform.localScale = new Vector3(5, 5, 1);
                break;
            default:
                break;
        }

        isEnable = true;
    }


    public void Show(RaycastHit hit, Transform player)
    {
        if (!isEnable) return;
        float skillRange = curSkillData.maxRange;
        switch (curSkillData.RangeType)
        {
            case RangeType.None:
                break;
            case RangeType.Point:
                indicatorImg.transform.position = hit.point;
                Debug.DrawLine(player.position, hit.point, Color.red);
                break;
            case RangeType.ArcShaped:
                break;
            case RangeType.Circle:
                float distance = Vector3.Distance(player.position, hit.point);
                var dir = hit.point - player.position;
                if (distance >= skillRange / 2.0f) distance = skillRange / 2.0f;
                Debug.DrawLine(player.position, player.position + dir.normalized * distance, Color.blue);
                indicatorImg.transform.position = player.position + dir.normalized * distance;
                break;
            case RangeType.DirectLine:
                indicatorImg.transform.position = hit.point;
                Debug.DrawLine(player.position, hit.point, Color.red);
                break;
            default:
                break;
        }
    }

    // 取消技能
    public void CancleSkill()
    {
        isEnable = false;
        indicatorImg.gameObject.SetActive(false);
        indicatorRangeImg.gameObject.SetActive(false);
    }

    public SkillData GetCurrentSkillData() {
        if (isEnable) {
            return curSkillData;
        }
        Debug.Log("UIIndicator 无法获取当前skillData数据");
        return null;
    }
}
