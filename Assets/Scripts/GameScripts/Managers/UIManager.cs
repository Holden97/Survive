using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 4.15之后所有的UI管理均在此写
/// </summary>
public class UIManager : MonoBehaviour
{
    GameObject player;
    public Image HpBar;
    public Text[] HintTextArray = new Text[5];//Canvas中的5个Text
    static Color hintColor;//提示信息的初始颜色
    public static List<string> hintList;//提示文本
    public static float showTime = 5f;
    // Start is called before the first frame update
    static UIManager instance;

    public static UIManager Instance { get => instance; set => instance = value; }

    void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        hintList = new List<string>();
        hintColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        hintList.Clear();
        foreach (var text in HintTextArray)
        {
            text.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        HPUpdate();
        HintUpdate();
    }
    /// <summary>
    /// 每帧更新HINT显示，主要需实现淡出效果
    /// </summary>
    private void HintUpdate()
    {
        if (hintList.Count != 0)
            for (int i = 0; i < Mathf.Min(hintList.Count - 1, HintTextArray.Length - 1); i++)
            {
                HintTextArray[i].text = hintList[i];
            }
        foreach (var text in HintTextArray)
        {
            float curAlpha = text.color.a;
            text.color = new Color(hintColor.r, hintColor.g, hintColor.b, curAlpha -= Time.deltaTime/showTime);
        }
        //说明此时仍可以移位
        if (HintTextArray[0].color.a <= 0 && hintList.Count > 0)
        {
            //移动现有位，remove hintList的第一项
            shiftText();
        }
    }

    void HPUpdate()
    {
        Image currentHP = HpBar.transform.Find("CurrentHP").gameObject.GetComponent<Image>();
        currentHP.fillAmount = PlayerController.Instance.curHealth / PlayerController.Instance.maxHealth;
        if (currentHP.fillAmount > 0.5)
            currentHP.color = Color.green;
        else if (currentHP.fillAmount > 0.3)
            currentHP.color = Color.yellow;
        else
            currentHP.color = Color.red;
    }

    internal void AddHint(string v)
    {
        hintList.Add(v);
        if (hintList.Count > 5)
        {
            //移动现有位，remove hintList的第一项
            shiftText();
            //原末位变为新项
            AddHint(hintList.Count - 1, v, 255 / 255f);

        }
        //原末位变为新项
        AddHint(hintList.Count - 1, v, 255 / 255f);

    }

    /// <summary>
    /// 当前现有文本列表前移一位，原末位复位
    /// 长度为hintList.Count
    /// </summary>
    void shiftText()
    {

        for (int i = 0; i < Mathf.Min(hintList.Count - 1, HintTextArray.Length - 1); i++)
        {
            AddHint(i, hintList[i + 1], HintTextArray[i + 1].color.a);
        }
        //最后一位复位
        AddHint(Mathf.Min(hintList.Count - 1, HintTextArray.Length - 1), "", 0 / 255f);
        hintList.RemoveAt(0);


    }


    /// <summary>
    /// 为指定index的Text添加指定alpha值的text
    /// </summary>
    /// <param name="index">0-4,Text位置</param>
    /// <param name="text">添加的内容</param>
    /// <param name="alpha">alpha值</param>
    void AddHint(int index, string text, float alpha)
    {
        HintTextArray[index].text = text;
        HintTextArray[index].color = new Color(hintColor.r, hintColor.g, hintColor.b, alpha);
    }
}
