using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillIndicator : MonoBehaviour
{
    Dictionary<int, Sprite> spriteAssetsDict; // 缓存resources
    Image indicatorImg;
    bool isEnable;

    void Start()
    {
        isEnable = false;
        spriteAssetsDict = new Dictionary<int, Sprite>();
        indicatorImg = transform.Find("indicator").GetComponent<Image>();
        indicatorImg.gameObject.SetActive(false);

        // 选择技能
        Character.OnSelectedSkill += (characterData, skillId) =>
        {
            var skillData = characterData.GetSkillData(skillId);
            this.SetData(skillData);
            isEnable = true;
        };

        // 取消技能
        Character.OnCancledSkill += (characterData, skillId) =>
        {
            this.CancleSkill();
        };

        Character.OnFiredSkill += (characterData, skillId) =>
        {
            this.CancleSkill();
        };
    }

    public void SetData(CharacterData.SkillData skillData)
    {
        Sprite sprite = null;
        int rangeType = (int)skillData.rangeType;
        spriteAssetsDict.TryGetValue(rangeType, out sprite);
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>(string.Format("Textures/indicator_{0}", rangeType));
            if (sprite == null)
            {
                Debug.Log(string.Format("无找到该rangeType={0}的sprite图片", rangeType));
                return;
            }
            spriteAssetsDict[rangeType] = sprite;
        }

        indicatorImg.sprite = sprite;
        indicatorImg.gameObject.SetActive(true);
        this.ConfigTest(skillData.rangeType);
    }
    
    // test
    void ConfigTest(CharacterData.SkillData.RangeType rangeType)
    {
        switch (rangeType)
        {
            case CharacterData.SkillData.RangeType.None:
                break;
            case CharacterData.SkillData.RangeType.Point:
                this.indicatorImg.transform.localScale = new Vector3(5, 5, 1);
                break;
            case CharacterData.SkillData.RangeType.ArcShaped:
                break;
            case CharacterData.SkillData.RangeType.Circle:
                break;
            case CharacterData.SkillData.RangeType.DirectLine:
                break;
            default:
                break;
        }
    }

    // 技能引导
    public void ShowIndicator(RaycastHit hit, Transform player)
    {
        if (!isEnable) return;

        this.indicatorImg.transform.position = hit.point;
        Debug.DrawLine(player.position, hit.point, Color.red);
    }

    void CancleSkill()
    {
        this.isEnable = false;
        this.indicatorImg.gameObject.SetActive(false);
    }
}
