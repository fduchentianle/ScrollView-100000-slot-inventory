using MSFrame;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowPanel<GamePanel>();
    }
}
