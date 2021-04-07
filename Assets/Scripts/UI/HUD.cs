using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private int healthMax;
    [SerializeField] private int healthValue;
    private bool isAnim;
    private float maxWidth;
    private Image healthImg;
    private RectTransform healthImgRect;
    public AnimationCurve animationCurve;
    private IAttackable owner;

    void Awake()
    {
        owner = MyUtility.GetComponentInParent<IAttackable>(transform);
        healthImgRect = MyUtility.GetComponent<RectTransform>(transform, "bg/health");
        healthImg = MyUtility.GetComponent<Image>(transform, "bg/health");
        animationCurve.AddKey(0, 0);
        animationCurve.AddKey(1, 1);
        animationCurve.preWrapMode = WrapMode.Default;
        maxWidth = healthImgRect.sizeDelta.x;
    }

    public void Init(int healthMax)
    {
        this.healthMax = healthMax;
        this.SetData(healthMax);
    }

    void Update()
    {
        if (owner != null && healthMax > 0)
        {
            if (owner.GetHP() != healthValue)
            {
                SetData(owner.GetHP());
            }
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.forward);
    }

    public void SetData(int healthValue_)
    {
        if (healthMax == 0) {
            Debug.LogError("healthMax 未赋值");
            return;
        }

        if (isAnim) return;
        isAnim = true;

        if (healthValue_ <= 0) {
            healthValue_ = 0;
        }

        StartCoroutine(UITween(healthValue_));
    }

    IEnumerator UITween(int endValue)
    {
        float imgWidth = maxWidth * endValue / healthMax;
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
        healthValue = endValue;
    }
}
