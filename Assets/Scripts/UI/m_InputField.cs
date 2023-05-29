using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class m_InputField : InputField
{
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (this.m_Keyboard == null)
        {
            return;
        }

        this.m_Keyboard.active = false;
        this.m_Keyboard = null;
    }
}
