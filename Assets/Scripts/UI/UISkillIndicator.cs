using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillIndicator : MonoBehaviour
{
    Dictionary<int, Sprite> spriteAssetsDict;
    Image indicatorImg;
    public bool IsEnable => isEnable;
    bool isEnable;
    public SkillData curSkillData;

    void Awake()
    {
        isEnable = false;
        spriteAssetsDict = new Dictionary<int, Sprite>();
        indicatorImg = MyUtility.GetComponent<Image>(transform, "indicator");
        indicatorImg.gameObject.SetActive(false);
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
    
        switch (RangeType)
        {
            case RangeType.None:
                break;
            case RangeType.Point:
                indicatorImg.transform.localScale = new Vector3(5, 5, 1);
                break;
            case RangeType.ArcShaped:
                break;
            case RangeType.Circle:
                break;
            case RangeType.DirectLine:
                indicatorImg.transform.localScale = new Vector3(5, 5, 1);
                break;
            default:
                break;
        }

        isEnable = true;
    }

    // 开始技能引导
    public void Show(RaycastHit hit, Transform player)
    {
        if (!isEnable) return;

        switch(curSkillData.RangeType)
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
    }

    public SkillData GetCurrentSkillData() {
        if (isEnable) {
            return curSkillData;
        }
        Debug.Log("UIIndicator 无法获取当前skillData数据");
        return null;
    }
}
