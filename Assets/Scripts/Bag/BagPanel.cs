using MSFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    //背包格子基础信息
    private const int ColumnCount = 3;
    private const float Padding = 20f;
    private const float CellSize = 100f;
    private const float Spacing = 10f;
    private const float CellStep = CellSize + Spacing;

    //viewport下的content对象
    public RectTransform content;
    private RectTransform viewport;

    //界面上显示的格子字典
    private Dictionary<int,GameObject> nowShowItems = new Dictionary<int, GameObject>();
    //要放入对象池的格子
    private List<int> removeIndex = new List<int>();
    //旧索引判断是否要更新
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;

    protected override void Awake()
    {
        base.Awake();
        ScrollRect scrollRect = content.GetComponentInParent<ScrollRect>();
        viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
    }

    public override void ShowMe()
    {
        //将content的高度设置为100000个格子的总高度
        content.sizeDelta = new Vector2(0, Padding + Mathf.CeilToInt(BagManager.Instance.items.Count / (float)ColumnCount) * CellStep);
        //显示格子
        CheckShoworHide();
    }

    public override void HideMe() { }

    private void Update()
    {
        CheckShoworHide();
    }

    /// <summary>
    /// 更新格子显示方法
    /// </summary>
    public void CheckShoworHide()
    {
        //检测哪些格子应该显示出来
        float viewportHeight = viewport.rect.height;
        int minIndex = Mathf.FloorToInt(Mathf.Max(0f, content.anchoredPosition.y - Padding) / CellStep) * ColumnCount;
        int maxIndex = Mathf.Min(Mathf.FloorToInt(Mathf.Max(0f, content.anchoredPosition.y + viewportHeight - Padding) / CellStep) * ColumnCount + ColumnCount - 1, BagManager.Instance.items.Count - 1);
        //如果没有拖动滚动条，索引不变 不用更新
        if (minIndex == oldMinIndex && maxIndex == oldMaxIndex)
            return;
        else
        {
            oldMinIndex = minIndex;
            oldMaxIndex = maxIndex;
        }

        //先将移出界面的格子放入对象池
        foreach (var pairs in nowShowItems)
        {
            if (pairs.Key < minIndex || pairs.Key > maxIndex)
            {
                removeIndex.Add(pairs.Key);
                PoolManager.Instance.PushGameObj(pairs.Value);
            }
        }

        for (int i = 0; i < removeIndex.Count; i++)
        {
            nowShowItems.Remove(removeIndex[i]);
        }

        removeIndex.Clear();

        //再加载即将显示的格子
        for (int index = minIndex; index <= maxIndex; index++)
        {
            //如果格子已经显示了，就不用加载
            if (nowShowItems.ContainsKey(index))
                continue;
            //没有显示的从对象池中拿
            BagItem item = PoolManager.Instance.GetGameObj<BagItem>("UI/BagItem");
            item.transform.SetParent(content);
            item.transform.localScale = Vector3.one;
            //注意：localposition是对象的pivot在父对象pivot下的偏移 anchoredPosition是对象的pivot在锚点下的偏移
            item.transform.localPosition = new Vector3(Padding + (index % ColumnCount) * CellStep, -Padding - (index / ColumnCount) * CellStep, 0);
            item.InitItemInfo(BagManager.Instance.items[index]);

            nowShowItems.Add(index, item.gameObject);
        }
    }
}
