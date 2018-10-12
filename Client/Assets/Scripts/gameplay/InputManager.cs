using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// 输入管理
/// </summary>
public class InputManager : MonoBehaviour
{
    // 移动增量
    private Vector2 _deltaMovement;

    // 转向增量
    private Vector2 _deltaRotation;

    // 玩家当前控制的角色
    private PlayerController _player;

    public PlayerController Player
    {
        get { return _player; }

        set { _player = value; }
    }

    void FixedUpdate()
    {
        if (SpaceBattle.Instance.isBattleStart)
        {
            // 每一帧都将移动增量和转向增量数据发往 Server
            SendShipInfoToServer();
        }
    }

    /// <summary>
    /// 处理左摇杆移动事件
    /// </summary>
    /// <param name="movement"></param>
    public void LeftJoystickMovement(Vector2 movement)
    {
        //        MMDebug.DebugOnScreen("left joystick", movement);

        if (SpaceBattle.Instance.isBattleStart)
        {
            _deltaMovement = movement;
//            _player.deltaMovement = movement;
        }
    }

    /// <summary>
    /// 处理右摇杆移动事件
    /// </summary>
    /// <param name="movement"></param>
    public void RightJoystickMovement(Vector2 movement)
    {
        //        MMDebug.DebugOnScreen("right joystick", movement);

        if (SpaceBattle.Instance.isBattleStart)
        {
            _deltaRotation = movement;
//            _player.deltaRotation = movement;
        }
    }

    /// <summary>
    /// 向 Server 发送飞船的位置和转向信息
    /// </summary>
    private void SendShipInfoToServer()
    {
        // 组装协议
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString(Constant.UpdateShipInfo);

        // 位置
        Vector2 mov = _deltaMovement;
        // 旋转
        Vector2 rot = _deltaRotation;

        proto.AddFloat(mov.x);
        proto.AddFloat(mov.y);
        proto.AddFloat(rot.x);
        proto.AddFloat(rot.y);

        // 向 Server 发送消息
        NetMgr.Instance.srvConn.Send(proto);
    }
}