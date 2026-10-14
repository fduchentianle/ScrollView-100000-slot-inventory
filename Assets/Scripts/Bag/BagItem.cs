using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MSFrame;

/// <summary>
/// 格子类对象 他是放在背包里的一个一个的道具格子
/// 主要用来显示 单组道具信息的
/// </summary>
[Pool(maxNum = 24)]
public class BagItem : BasePanel
{
    public void InitItemInfo(Item info)
    {
        GetControl<TextMeshProUGUI>("txtNum").text = info.num.ToString();
    }
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
