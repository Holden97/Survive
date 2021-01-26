using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 公共输入模块
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    bool isOpen = false;
    public InputMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(InputUpdate);
    }

    private void InputUpdate()
    {
        if (!isOpen)
            return;
        CheckKeyState(KeyCode.W);
        CheckKeyState(KeyCode.S);
        CheckKeyState(KeyCode.A);
        CheckKeyState(KeyCode.D);
    }

    /// <summary>
    /// 控制输入检测的开启与关闭
    /// </summary>
    private void ControlSwitch(bool isOpen)
    {
        this.isOpen = isOpen;
    }
    private void CheckKeyState(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            EventCenter.GetInstance().EventTrigger("键按下", key);
        }
        if (Input.GetKeyUp(key))
        {
            EventCenter.GetInstance().EventTrigger("键抬起", key);
        }
    }
}
