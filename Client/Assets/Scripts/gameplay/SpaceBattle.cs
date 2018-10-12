using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 太空战斗场景管理
/// </summary>
public class SpaceBattle
{
    private static SpaceBattle _instance;

    /// <summary>
    /// 获取 SpaceBattle 单例
    /// </summary>
    public static SpaceBattle Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new SpaceBattle();
            }
            return _instance;
        }
    }

    // 房间中的所有玩家 ID 列表
    public Dictionary<int, string> PlayerDict = new Dictionary<int, string>();

    // 场景中的所有飞船
    public Dictionary<string, Ship> ShipDict = new Dictionary<string, Ship>();

    // 战斗是否开始
    public bool isBattleStart = false;

    // TODO: 接收到 Server 发送数据的事件回调。这里跟底层消息分发的事件回调机制有所重复，应该可以再精简去掉一次回调
    public delegate void EventCallback(byte eventCode, object content, int senderId);

    public static EventCallback OnEventCall;

    /// <summary>
    /// 私有构造方法，防止单例模式下产生多个类的实例
    /// </summary>
    private SpaceBattle()
    {
        // nothing to do here.
    }

    /// <summary>
    /// 开始战斗
    /// </summary>

    public void StartBattle()
    {
        // 处理每一架飞船
        for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
        {
            //KBEngine.Avatar avatar = GameData.Instance.RoomPlayers[i];

            // 产生飞船
            //SpawnShip(avatar);
        }


        // 向消息分发管理器注册接收到 TrueSync 同步数据的回调方法

        KBEngine.Event.registerOut("onTrueSyncData", this, "onTrueSyncData");


        isBattleStart = true;
    }

    /// <summary>
    /// 处理接收到的 TrueSync 消息
    /// </summary>
    public void onTrueSyncData(KBEngine.Entity entity, byte eventCode, byte[] message)
    {
        // 解析协议
        //         int start = 0;
        //         ProtocolBytes proto = (ProtocolBytes)protocol;
        //         // 协议名称（暂时没用，但也要取出来）
        //         string unused = proto.GetString(start, ref start);
        //         byte eventCode = proto.GetByte(start, ref start);
        //         byte[] data = proto.GetBytes(start, ref start);
        // TODO: 事件编码和发送玩家 ID 暂时写死（协议内容中可以解析到每个指令的玩家 ID）
        //OnEventCall(eventCode, data, -1);

        Debug.Log("SpaceBattle.onTrueSyncData,entiy." + entity.id + ",eventCode:" + eventCode + ",message:" + message+ ",Length:" + message.Length);
        //byte[] data = System.Text.Encoding.Default.GetBytes(message);
        object content = Trans.Bytes2Object(message);
        //object content = data as object;

        OnEventCall(eventCode, content, entity.id);
    }

    #region Obsolete Code

    /// <summary>
    /// 在场景中生成飞船
    /// </summary>
    /// <param name="id"></param>
    /// <param name="team"></param>
    /// <param name="spid"></param>
    private void SpawnShip(KBEngine.Avatar avatar)
    {
        // 获取预设的出生点
        Transform spawnPoints = GameObject.FindWithTag("SpwanPoints").transform;
        // 出生点坐标
        Transform spawnTrans;
        int team = 1;
        int spid = avatar.isPlayer() ? 0 : 1;

        if (team == 1)
        {
            spawnTrans = spawnPoints.GetChild(0).GetChild(spid - 1);
        }
        else
        {
            spawnTrans = spawnPoints.GetChild(1).GetChild(spid - 1);
        }

        if (spawnTrans == null)
        {
            Debug.LogError("SpawnShip 出生点错误！");
            return;
        }

        // 获取飞船 prefab
        GameObject battleControllerObject = GameObject.FindWithTag("BattleController");
        if (battleControllerObject == null)
        {
            Debug.LogError("场景中缺失 BattleController 对象！");
            return;
        }

        BattleController battleController = battleControllerObject.GetComponent<BattleController>();
        GameObject[] shipPrefabs = battleController.characterPrefabs;

        if (shipPrefabs.Length < 2)
        {
            Debug.LogError("飞船预设数量不够！");
            return;
        }

        // 产生飞船
        GameObject shipObj = (GameObject) Object.Instantiate(shipPrefabs[team - 1]);
        shipObj.name = avatar.id.ToString();
        shipObj.transform.position = spawnTrans.position;
        shipObj.transform.rotation = spawnTrans.rotation;

        // 构造 Ship 对象
        Ship ship = new Ship();
        ship.playerController = shipObj.GetComponent<PlayerController>();
        ship.team = team;

        // 保存到场景的 Ship 列表中
        ShipDict.Add(avatar.id.ToString(), ship);

        // 角色操控处理
//        if (id == GameMgr.instance.id)
//        {
//            ship.playerController.ControlMode = PlayerController.CharacterControlMode.Player;
//
//            // 设置摇杆操作控制的角色
//            GameObject inputManagerObject = GameObject.FindWithTag("InputManager");
//            if (inputManagerObject == null)
//            {
//                Debug.LogError("场景中缺失 InputManager 对象！");
//                return;
//            }
//
//            InputManager inputManager = inputManagerObject.GetComponent<InputManager>();
//            inputManager.Player = ship.playerController;
//        }
//        else
//        {
//            ship.playerController.ControlMode = PlayerController.CharacterControlMode.Net;
//        }
    }

    /// <summary>
    /// 处理接收到的单位同步信息
    /// </summary>
    private void RecvUpdateShipInfo(ProtocolBase protocol)
    {
        // 解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        // 协议名称（暂时没用，但也要取出来）
        string unused = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);

        // 如果该位置同步消息包是自己发出的，则忽略，不进行位置和转向同步，避免被拉回到之前的位置上
//        if (id == GameMgr.instance.id)
//        {
//            return;
//        }

        Vector2 mov;
        Vector2 rot;
        // 移动向量
        mov.x = proto.GetFloat(start, ref start);
        mov.y = proto.GetFloat(start, ref start);
        // 转向向量
        rot.x = proto.GetFloat(start, ref start);
        rot.y = proto.GetFloat(start, ref start);

        // 处理
        Debug.Log("RecvUpdateShipInfo - " + id);
        if (!ShipDict.ContainsKey(id))
        {
            Debug.Log("RecvUpdateShipInfo ship == null ");
            return;
        }

        // 设置飞船的移动和转向
        Ship ship = ShipDict[id];
        ship.playerController.deltaMovement = mov;
        ship.playerController.deltaRotation = rot;
    }

    #endregion
}