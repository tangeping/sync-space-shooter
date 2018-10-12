using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiBattle : MonoBehaviour
{
    // 单例
    public static MultiBattle instance;

    // 坦克预设
    public GameObject[] tankPrefabs;

    // 战场中的所有坦克
    public Dictionary<string, BattleTank> list = new Dictionary<string, BattleTank>();

    void Start()
    {
        // 单例模式
        instance = this;
    }

    // 获取阵营 0表示错误
    public int GetCamp(GameObject tankObj)
    {
        foreach (BattleTank mt in list.Values)
        {
            if (mt.tank.gameObject == tankObj)
            {
                return mt.camp;
            }
        }
        return 0;
    }

    /// <summary>
    /// 判断两个单位是否同一阵营
    /// </summary>
    /// <param name="tank1"></param>
    /// <param name="tank2"></param>
    /// <returns></returns>
    public bool IsSameCamp(GameObject tank1, GameObject tank2)
    {
        return GetCamp(tank1) == GetCamp(tank2);
    }

    /// <summary>
    /// 清理场景
    /// </summary>
    public void ClearBattle()
    {
        list.Clear();

        // TODO: 这里使用了 Tank 标签来获取所有单位
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");

        for (int i = 0; i < tanks.Length; i++)
        {
            Destroy(tanks[i]);
        }
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    /// <param name="proto"></param>
    public void StartBattle(ProtocolBytes proto)
    {
        // 解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "Fight")
        {
            return;
        }

        // 坦克总数
        int count = proto.GetInt(start, ref start);

        // 清理场景
        ClearBattle();

        // 每一辆坦克
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int team = proto.GetInt(start, ref start);
            int swopID = proto.GetInt(start, ref start);

            // 生成坦克
            GenerateTank(id, team, swopID);
        }

        // 向消息分发管理器注册相应事件的回调方法
        NetMgr.Instance.srvConn.msgDist.AddListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        NetMgr.Instance.srvConn.msgDist.AddListener("Shooting", RecvShooting);
        NetMgr.Instance.srvConn.msgDist.AddListener("Hit", RecvHit);
        NetMgr.Instance.srvConn.msgDist.AddListener("Result", RecvResult);
    }

    /// <summary>
    /// 生成坦克
    /// </summary>
    /// <param name="id"></param>
    /// <param name="team"></param>
    /// <param name="swopID"></param>
    public void GenerateTank(string id, int team, int swopID)
    {
        // 获取预设的出生点
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform swopTrans;

        if (team == 1)
        {
            Transform teamSwop = sp.GetChild(0);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        else
        {
            Transform teamSwop = sp.GetChild(1);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }

        if (swopTrans == null)
        {
            Debug.LogError("GenerateTank出生点错误！");
            return;
        }

        // 预设
        if (tankPrefabs.Length < 2)
        {
            Debug.LogError("坦克预设数量不够");
            return;
        }

        // 产生坦克
        GameObject tankObj = (GameObject) Instantiate(tankPrefabs[team - 1]);
        tankObj.name = id;
        tankObj.transform.position = swopTrans.position;
        tankObj.transform.rotation = swopTrans.rotation;

        // 列表处理
        BattleTank bt = new BattleTank();
        bt.tank = tankObj.GetComponent<Tank>();
        bt.camp = team;
        list.Add(id, bt);

        // 玩家处理
        if (id == GameMgr.instance.id)
        {
            bt.tank.ctrlType = Tank.CtrlType.player;
            CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
            GameObject target = bt.tank.gameObject;
            cf.SetTarget(target);
        }
        else
        {
            bt.tank.ctrlType = Tank.CtrlType.net;
            bt.tank.InitNetCtrl(); //初始化网络同步
        }
    }

    /// <summary>
    /// 处理接收到的单位同步信息
    /// </summary>
    /// <param name="protocol"></param>
    public void RecvUpdateUnitInfo(ProtocolBase protocol)
    {
        // 解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);

        Vector3 nPos;
        Vector3 nRot;
        // 位置向量
        nPos.x = proto.GetFloat(start, ref start);
        nPos.y = proto.GetFloat(start, ref start);
        nPos.z = proto.GetFloat(start, ref start);
        // 转向向量
        nRot.x = proto.GetFloat(start, ref start);
        nRot.y = proto.GetFloat(start, ref start);
        nRot.z = proto.GetFloat(start, ref start);

        float turretY = proto.GetFloat(start, ref start);
        float gunX = proto.GetFloat(start, ref start);

        // 处理
        Debug.Log("RecvUpdateUnitInfo " + id);
        if (!list.ContainsKey(id))
        {
            Debug.Log("RecvUpdateUnitInfo bt == null ");
            return;
        }

        BattleTank bt = list[id];

        // 如果该位置同步消息包是自己发出的，则忽略，不进行位置和转向同步，避免被拉回到之前的位置上
        if (id == GameMgr.instance.id)
        {
            return;
        }

        // 设置坦克的位置和转向
        bt.tank.NetForecastInfo(nPos, nRot);
        bt.tank.NetTurretTarget(turretY, gunX); // 稍后实现
    }

    /// <summary>
    /// 处理接收到的开火同步信息
    /// </summary>
    /// <param name="protocol"></param>
    public void RecvShooting(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        Vector3 pos;
        Vector3 rot;
        pos.x = proto.GetFloat(start, ref start);
        pos.y = proto.GetFloat(start, ref start);
        pos.z = proto.GetFloat(start, ref start);
        rot.x = proto.GetFloat(start, ref start);
        rot.y = proto.GetFloat(start, ref start);
        rot.z = proto.GetFloat(start, ref start);
        //处理
        if (!list.ContainsKey(id))
        {
            Debug.Log("RecvShooting bt == null ");
            return;
        }
        BattleTank bt = list[id];
        if (id == GameMgr.instance.id)
        {
            return;
        }
        bt.tank.NetShoot(pos, rot);
    }


    public void RecvHit(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        string protoName = proto.GetString(start, ref start);
        string attId = proto.GetString(start, ref start);
        string defId = proto.GetString(start, ref start);
        float hurt = proto.GetFloat(start, ref start);
        //获取BattleTank
        if (!list.ContainsKey(attId))
        {
            Debug.Log("RecvHit attBt == null " + attId);
            return;
        }
        BattleTank attBt = list[attId];

        if (!list.ContainsKey(defId))
        {
            Debug.Log("RecvHit defBt == null " + defId);
            return;
        }
        BattleTank defBt = list[defId];
        //被击中的坦克
        defBt.tank.NetBeAttacked(hurt, attBt.tank.gameObject);
    }


    public void RecvResult(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        string protoName = proto.GetString(start, ref start);
        int winTeam = proto.GetInt(start, ref start);
        //弹出胜负面板
        string id = GameMgr.instance.id;
        BattleTank bt = list[id];
        if (bt.camp == winTeam)
        {
            PanelMgr.instance.OpenPanel<WinPanel>("", 1);
        }
        else
        {
            PanelMgr.instance.OpenPanel<WinPanel>("", 0);
        }
        //取消监听
        NetMgr.Instance.srvConn.msgDist.DelListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        NetMgr.Instance.srvConn.msgDist.DelListener("Shooting", RecvShooting);
        NetMgr.Instance.srvConn.msgDist.DelListener("Hit", RecvHit);
        NetMgr.Instance.srvConn.msgDist.DelListener("Result", RecvResult);
    }
}