using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private HealthData healthData;
    private bool isAnim;
    private float maxWidth;
    private RectTransform healthImgRect;
    public AnimationCurve animationCurve;
    private IAttackable owner;

    void Awake()
    {
        owner = MyUtility.GetComponentInParent<IAttackable>(transform);
        healthImgRect = MyUtility.GetComponent<RectTransform>(transform, "bg/health");
        animationCurve.AddKey(0, 0);
        animationCurve.AddKey(1, 1);
        animationCurve.preWrapMode = WrapMode.Default;
        maxWidth = healthImgRect.sizeDelta.x;
    }

    public void Init(HealthData healthData_)
    {
        healthData = healthData_;
        SetData(healthData.HP.max);
    }

    void Update()
    {
        if (owner == null) return;
        if (healthData.GetMaxHP() <= 0) return;
        if (owner.GetHP() != healthData.HP.value)
        {
            SetData(owner.GetHP());
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.forward);
    }

    public void SetData(int hp)
    {
        int maxHP = healthData.GetMaxHP();
        if (maxHP == 0) {
            Debug.LogError("healthMax 未赋值");
            return;
        }

        if (isAnim) return;
        isAnim = true;

        if (hp <= 0) {
            hp = 0;
        }

        StartCoroutine(UITween(hp));
    }

    IEnumerator UITween(int endValue)
    {
        int maxHP = healthData.GetMaxHP();
        float imgWidth = maxWidth * endValue / maxHP;
        Vector2 toSizeDelta = new Vector2(imgWidth, healthImgRect.sizeDelta.y);
        float startTime = Time.realtimeSinceStartup;
        float endTime = Time.realtimeSinceStartup + 1;
        while (Time.realtimeSinceStartup <= endTime)
        {
            float passTime = Time.realtimeSinceStartup - startTime;
            healthImgRect.sizeDelta = Vector2.Lerp(healthImgRect.sizeDelta, toSizeDelta, animationCurve.Evaluate(passTime));
            yield return new WaitForSeconds(0.02f);
        }
        isAnim = false;
        healthData.SetHP(endValue);
    }
}
