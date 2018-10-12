using UnityEngine;
using System.Collections;

public class Battle : MonoBehaviour
{
    //单例
    public static Battle instance;
    //战场中的所有坦克
    public BattleTank[] battleTanks;
    //坦克预设
    public GameObject[] tankPrefabs;

    //开始
    void Start()
    {
        //单例
        instance = this;
        //开始战斗
        //StartTwoCampBattle(1, 2);
    }

    //获取阵营 0表示错误
    public int GetCamp(GameObject tankObj)
    {
        for (int i = 0; i < battleTanks.Length; i++)
        {
            BattleTank battleTank = battleTanks[i];
            if (battleTanks == null)
                return 0;
            if (battleTank.tank.gameObject == tankObj)
                return battleTank.camp;
        }
        return 0;
    }

    //是否同一阵营
    public bool IsSameCamp(GameObject tank1, GameObject tank2)
    {
        return GetCamp(tank1) == GetCamp(tank2);
    }

    //胜负判断
    public bool IsWin(int camp)
    {

        for (int i = 0; i < battleTanks.Length; i++)
        {
            Tank tank = battleTanks[i].tank;
            if (battleTanks[i].camp != camp)
                if (tank.hp > 0)
                    return false;
        }
        Debug.Log("阵营" + camp + "获胜");
        PanelMgr.instance.OpenPanel<WinPanel>("", camp);
        return true;
    }

    //胜负判断
    public bool IsWin(GameObject attTank)
    {
        int camp = GetCamp(attTank);
        return IsWin(camp);
    }

    //清理场景
    public void ClearBattle()
    {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < tanks.Length; i++)
            Destroy(tanks[i]);
    }

    //开始战斗
    public void StartTwoCampBattle(int n1, int n2)
    {
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform spCamp1 = sp.GetChild(0);
        Transform spCamp2 = sp.GetChild(1);
        //判断
        if (spCamp1.childCount < n1 || spCamp2.childCount < n2)
        {
            Debug.LogError("出生点数量不够");
            return;
        }
        if (tankPrefabs.Length < 2)
        {
            Debug.LogError("坦克预设数量不够");
            return;
        }
        //清理场景
        ClearBattle();
        //产生坦克
        battleTanks = new BattleTank[n1 + n2];
        for (int i = 0; i < n1; i++)
        {
            GenerateTank(1, i, spCamp1, i);

        }
        for (int i = 0; i < n2; i++)
        {
            GenerateTank(2, i, spCamp2, n1+i);
        }
        //把第一辆坦克设为玩家操控
        Tank tankCmp = battleTanks[0].tank;
        tankCmp.ctrlType = Tank.CtrlType.player;
        CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
        GameObject target = tankCmp.gameObject;
        cf.SetTarget(target);
    }


    //生成一辆坦克
    public void GenerateTank(int camp, int num, Transform spCamp, int index)
    {
        //获取出生点和预设
        Transform trans = spCamp.GetChild(num);
        Vector3 pos = trans.position;
        Quaternion rot = trans.rotation;
        GameObject prefab = tankPrefabs[camp-1];
        //产生坦克
        GameObject tankObj = (GameObject)Instantiate(prefab, pos, rot);
        //设置属性
        Tank tankCmp = tankObj.GetComponent<Tank>();
        tankCmp.ctrlType = Tank.CtrlType.computer;
        //battleTanks
        battleTanks[index] = new BattleTank();
        battleTanks[index].tank = tankCmp;
        battleTanks[index].camp = camp;
    }
}