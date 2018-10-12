using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 网络管理
/// </summary>
public class NetMgr
{
    private static NetMgr _instance;

    public Connection srvConn = new Connection();

    /// <summary>
    /// 获取 NetMgr 实例
    /// </summary>
    public static NetMgr Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new NetMgr();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 私有构造方法，防止单例模式下产生多个类的实例
    /// </summary>
    private NetMgr()
    {
        // nothing to do here.
    }

    public void Start()
    {
        // 向消息分发管理器注册接收到 Ping 测试消息的回调方法
        srvConn.msgDist.AddListener(Constant.Ping, RecvPingProtocol);
    }

    public void Update()
    {
        // 驱动消息分发、报告心跳、测试延迟等网络相关处理
        srvConn.Update();
    }

    // 心跳
    public ProtocolBase GetHeartBeatProtocol()
    {
        //具体的发送内容根据服务端设定改动
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString(Constant.HeartBeat);

        return protocol;
    }

    /// <summary>
    /// 获取 Ping 协议消息
    /// </summary>
    /// <returns></returns>
    public ProtocolBase GetPingProtocol()
    {
        ProtocolBytes proto = new ProtocolBytes();

        proto.AddString(Constant.Ping);
        // 添加当前时间（游戏开始到现在的时间）
        proto.AddFloat(Time.time);

        return proto;
    }

    /// <summary>
    /// 处理 Ping 协议消息
    /// </summary>
    private void RecvPingProtocol(ProtocolBase protocol)
    {
        // 解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        // 协议名称（暂时没用，但也要取出来）
        string unused = proto.GetString(start, ref start);
        float time = proto.GetFloat(start, ref start);

        // 根据 Ping 消息中的发送时间，计算出网络往返时间（延迟），并更新网络状态 NetStatus
        NetStatus.Instance.AddRtt(Time.time - time);
    }
}