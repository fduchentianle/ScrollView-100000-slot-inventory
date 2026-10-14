# 100000 个格子的背包

一个基于 Unity UGUI 的**十万格子虚拟滚动背包**实现。背包有 100,000 个格子，但界面上**同一时刻只实例化约 21 个格子的对象**，其余全部走对象池复用——滚动时按可视区域动态计算索引区间，回收移出视野的格子、补上进入视野的格子。

## 核心思路

朴素的背包做法是给每个格子 `Instantiate` 一个 GameObject。10 万个格子的场景下这会产生 10 万个对象，内存和布局开销都无法接受。

本项目的做法是**虚拟滚动（virtual scrolling / 按需渲染）**：

1. `content` 的高度按总格子数一次性算好，让滚动条有正确的滚动范围（这一步不创建任何对象）
2. 每帧根据 `content.anchoredPosition.y` 和 viewport 高度，算出当前应该可见的**索引区间** `[minIndex, maxIndex]`
3. 区间内尚未显示的格子从对象池取出、定位、填数据；已移出区间的格子回收到对象池
4. 索引区间没变就提前 `return`，不做任何无意义的重算

因此滚动条长度代表 10 万格子的真实规模，而实际对象数始终维持在可视区域所需的数量级。

## 关键实现

| 文件 | 职责 |
|---|---|
| `Assets/Scripts/Bag/BagManager.cs` | 数据层。单例，持有 `List<Item>`，初始化 100,000 条道具数据 |
| `Assets/Scripts/Bag/BagPanel.cs` | 视图层。虚拟滚动的核心：区间计算、格子复用、content 高度设置 |
| `Assets/Scripts/Bag/BagItem.cs` | 单个格子。标记 `[Pool(maxNum = 24)]`，从池中取出后填充显示 |
| `Assets/Scripts/Main.cs` | 入口。启动时打开 `GamePanel` |
| `Assets/Scripts/Panel/GamePanel.cs` | 承载背包界面的面板 |
| `Assets/Scripts/MSFrame/` | 自研轻量框架：单例管理器、对象池、UI 面板栈、资源加载 |

### 布局参数

`BagPanel` 中的格子布局由常量控制：

```csharp
private const int   ColumnCount = 3;     // 每行格子数
private const float Padding     = 20f;   // 内边距
private const float CellSize    = 100f;  // 格子边长
private const float Spacing     = 10f;   // 格间距
```

### 索引区间计算

```csharp
int minIndex = Mathf.FloorToInt(Mathf.Max(0f, content.anchoredPosition.y - Padding) / CellStep) * ColumnCount;
int maxIndex = Mathf.Min(
    Mathf.FloorToInt(Mathf.Max(0f, content.anchoredPosition.y + viewportHeight - Padding) / CellStep) * ColumnCount + ColumnCount - 1,
    BagManager.Instance.items.Count - 1);
```

`CellStep = CellSize + Spacing`，先由滚动偏移算出"第几行"，再乘以 `ColumnCount` 换算成格子索引，`maxIndex` 额外补一整行并夹取到数据上限。

## 环境要求

| 项目 | 版本 |
|---|---|
| Unity | 2022.3.59f1c1 |
| TextMeshPro | 3.0.7 |
| 渲染管线 | 内置（Built-in） |

打开步骤：用 Unity Hub 添加本目录 → 用上述版本打开 → 打开 `Assets/Scenes/SampleScene.unity` → 运行。

## 关于仓库中不包含的内容

- `Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、`.vs/` 以及 `*.csproj` / `*.sln` —— Unity 与 IDE 自动生成的中间产物，已由 `.gitignore` 排除，首次打开项目时 Unity 会自行重建。
- **Odin Inspector（`Assets/Plugins/Sirenix`）** —— 这是付费商业插件，不可随公开仓库分发，因此未纳入版本控制。本项目的业务代码并未引用它；如果你的本地工程需要它，请自行购买并从 Asset Store 导入到 `Assets/Plugins/Sirenix`。

## 说明

这是用于学习 Unity 性能优化与 UGUI 滚动列表实现的练习项目，重点在于虚拟列表 + 对象池这一组合，而非完整的游戏背包功能（没有物品图标、拖拽、排序、存档等）。

## License

尚未指定。如需他人使用，建议补充一个 LICENSE 文件（例如 MIT）。
