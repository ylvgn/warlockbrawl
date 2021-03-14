using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelItem : MonoBehaviour
{
    Image bgImg;
    Image fillImg;
    Button btn;

    private int skillId = -1;

    void Awake()
    {
        btn = GetComponent<Button>();
        bgImg = transform.Find("bg").GetComponent<Image>();
        fillImg = transform.Find("fill").GetComponent<Image>();
        fillImg.fillAmount = 1;
        btn.onClick.AddListener(OnBtnClick);
    }

    public void Init(int skillId)
    {
        this.skillId = skillId;
        Sprite sprite = Resources.Load<Sprite>(string.Format("Textures/skill_{0}", skillId));
        bgImg.sprite = sprite;
        fillImg.sprite = sprite;
        fillImg.color = Color.white;
    }

    private void OnBtnClick()
    {
        if (skillId == -1) return; // 空技能槽不触发选择
        SendMessageUpwards("OnSelectSkill", skillId);
        Debug.Log(string.Format("click skillId = {0}", skillId));
    }

    public void CoolDown (float coolDownTime)
    {
        if (coolDownTime <= 0) coolDownTime = 0.01f; // be safe, division 0
        StartCoroutine(CDTween(coolDownTime));
    }

    IEnumerator CDTween(float coolDownTime)
    {
        fillImg.fillAmount = 0;
        var startTime = Time.realtimeSinceStartup;
        var endTime = startTime + coolDownTime;
        while (Time.realtimeSinceStartup <= endTime)
        {
            float passTime = Time.realtimeSinceStartup - startTime;
            fillImg.fillAmount = passTime / coolDownTime;
            yield return new WaitForSeconds(0.02f);
        }
        SendMessageUpwards("OnCoolDownOK", skillId);
        Debug.Log(string.Format("coolDownFinish skillId = {0}, cd耗时={1}", skillId, Time.realtimeSinceStartup - startTime));
        fillImg.fillAmount = 1;
    }
}
