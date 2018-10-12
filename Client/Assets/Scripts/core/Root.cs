using UnityEngine;

/// <summary>
/// 挂载到场景中的游戏入口脚本
/// </summary>
public class Root : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
        Application.runInBackground = true;

        // 开启网络
        NetMgr.Instance.Start();

        // 打开登录面板
        PanelMgr.instance.OpenPanel<LoginPanel>("");

        Debug.Log("=== Game has been started up! ===");
    }

    void FixedUpdate()
    {
        // 固定时间间隔更新，处理消息队列中的消息
        NetMgr.Instance.Update();
    }
}