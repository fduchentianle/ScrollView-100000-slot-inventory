using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具信息
/// </summary>
public class Item
{
    public int id;
    public int num;
}

public class BagManager : BaseManager<BagManager>
{
    public List<Item> items = new List<Item>();

    private BagManager()
    {
        if (items.Count == 0)
            InitItemInfo();
    }

    public void InitItemInfo()
    {
        for (int i = 0; i < 100000; i++)
        {
            Item item = new Item();
            item.id = i;
            item.num = i;

            items.Add(item);
        }
    }
}
