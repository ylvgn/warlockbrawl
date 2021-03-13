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

    void Start()
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
        Debug.Log(string.Format("click skillId = {0}", skillId));
        if (skillId == -1) 
            return;
        SendMessageUpwards("OnSelectSkill", skillId);
    }

    public void CoolDown (float coolDownTime)
    {
        if (coolDownTime <= 0) coolDownTime = 0.01f;
        StartCoroutine(CDTween(coolDownTime));
    }

    IEnumerator CDTween(float coolDownTime)
    {
        fillImg.fillAmount = 0;
        var endTime = Time.realtimeSinceStartup + coolDownTime;
        while (Time.realtimeSinceStartup < endTime)
        {
            fillImg.fillAmount += 1 / coolDownTime * Time.deltaTime;
            yield return new WaitForSeconds(0.02f);
        }
        SendMessageUpwards("OnCoolDownOK", skillId);
        fillImg.fillAmount = 1;
    }
}
