using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInputModule : StandaloneInputModule
{
    public MouseButtonEventData GetMouseEventData(PointerEventData.InputButton button)
    {
        var st = base.GetMousePointerEventData().GetButtonState(button);
        return st.eventData;
    }

    public override void UpdateModule()
    {
        base.UpdateModule();
        MyInput(PointerEventData.InputButton.Right); // 右键
    }

    void MyInput(PointerEventData.InputButton button)
    {
        if (PlayerController.Instance == null) {
            return;
        }

        var ev = GetMouseEventData(button).buttonData;
        if (ev.pointerEnter != null && ev.pointerEnter.layer == LayerMask.NameToLayer("UI"))
        {
            PlayerController.Instance.canClickMouse = false;
        } else PlayerController.Instance.canClickMouse = true;

    }
}
