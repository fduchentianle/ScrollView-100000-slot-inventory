using MSFrame;

public class GamePanel : BasePanel
{
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "BagBtn":
                UIManager.Instance.ShowPanel<BagPanel>();
                break;
        }
    }

    public override void HideMe() { }

    public override void ShowMe() { }
}
