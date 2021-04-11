using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillPanelItem : MonoBehaviour
{
    Image m_bgImg;
    Image m_fillImg;
    Button m_skillBtn;
    private int skillId;
    private bool isEnable;
    private float coolDownTime;
    public Action<int> SetClickCallBack;

    private bool isAnim;

    void Awake()
    {
        m_skillBtn = MyUtility.GetComponent<Button>(transform);
        m_bgImg = MyUtility.GetComponent<Image>(transform, "bg");
        m_fillImg = MyUtility.GetComponent<Image>(transform, "fill");
        m_fillImg.fillAmount = 1;
        m_skillBtn.onClick.AddListener(OnBtnClick);
    }

    public void Init(SkillData skillData)
    {
        skillId = skillData.id;
        coolDownTime = skillData.coolDownTime;
        Sprite sprite = Resources.Load<Sprite>(string.Format("Textures/skill_{0}", skillId));
        m_bgImg.sprite = sprite;
        m_fillImg.sprite = sprite;
        m_fillImg.color = Color.white;
        isAnim = false;
        isEnable = true;
    }

    public void Update()
    {
        if (!isEnable) return;
        if (PlayerController.Instance != null && PlayerController.Instance.character != null) {
            var character = PlayerController.Instance.character;
            if (character.IsSkillCoolDowning(skillId)) {
                if (!isAnim) {
                    isAnim = true;
                    m_fillImg.fillAmount = 0;
                }
                float passTime = Time.realtimeSinceStartup - character.GetCoolDownStartTimeStamp(skillId);
                m_fillImg.fillAmount = passTime / coolDownTime;
            } else if (isAnim) {
                m_fillImg.fillAmount = 1;
                isAnim = false;
            }
        }
    }

    public void OnBtnClick()
    {
        if (skillId == 0) return; // 空技能槽不触发选择
        if (SetClickCallBack != null && !isAnim)
            SetClickCallBack(skillId);
    }

    public int GetSkillId()
    {
        return skillId;
    }
}
