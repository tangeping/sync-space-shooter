using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KBEngine;
using System;

public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;
    private bool PannelInit = false;

    #region 生命周期

    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
    }

    public void Start()
    {
        KBEngine.Event.registerOut("onLeaveRoomResult", this, "onLeaveRoomResult");
        KBEngine.Event.registerOut("onGameBeginResult", this, "onGameBeginResult");
    }

    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //组件
        for (int i = 0; i < 6; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(name);
            //prefab.gameObject.SetActive(false);
            prefabs.Add(prefab);
        }

        if (GameData.Instance.IsCreater())
        {
            closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
            startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
            //按钮事件
            closeBtn.onClick.AddListener(OnCloseClick);
            startBtn.onClick.AddListener(OnStartClick);
        }
        //监听
        //NetMgr.Instance.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        //NetMgr.Instance.srvConn.msgDist.AddListener("Fight", RecvFight);
        //发送查询
        
    }

    public override void OnClosing()
    {
        OnDestroy();
    }

    public void RecvGetRoomInfo()
    {
        Debug.Log(" RoomPanel::RecvGetRoomInfo." + GameData.Instance.RoomPlayers.Count);

        int i = 0;
        for(; i< GameData.Instance.RoomPlayers.Count;i++)
        {            
            KBEngine.Avatar avatar = GameData.Instance.RoomPlayers[i];       
            Int32 win = avatar.component1.winScore;
            Int32 fail = avatar.component1.lossScore;
            Debug.Log(" avatar." + avatar.id + ",isPlayer:" + avatar.isPlayer());

            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            string str = "名字：" + avatar.id + "\r\n";
            str += "阵营：" + (!avatar.isPlayer() ? "红" : "蓝") + "\r\n";
            str += "胜利：" + win.ToString() + "   ";
            str += "失败：" + fail.ToString() + "\r\n";
            if (avatar.isPlayer())
                str += "【我自己】";
            if (avatar.id == GameData.Instance.CurrentRoom.room_creater)
                str += "【房主】";
            text.text = str;

            if (avatar.isPlayer())
                trans.GetComponent<Image>().color = Color.blue;
            else
                trans.GetComponent<Image>().color = Color.red;

        }

        for (; i < 6; i++)
        {
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            text.text = "【等待玩家】";
            trans.GetComponent<Image>().color = Color.gray;
        }

    }

    public void OnCloseClick()
    {
        KBEngine.Event.fireIn("reqLeaveRoom", new object[] { });
    }


    public void onLeaveRoomResult(byte result)
    {
        //处理
        if (result == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功!");
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }


    public void OnStartClick()
    {
        KBEngine.Event.fireIn("reqGameBegin", new object[] { });
    }

    public void onGameBeginResult(Int32 entityID, byte result)
    {
        //处理
        if (result != 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "开始游戏失败！两队至少都需要一名玩家，只有队长可以开始战斗！");
        }

        RecvFight();
    }

    /// <summary>
    /// 接收到‘开始战斗’消息
    /// </summary>

    public void RecvFight()
    {
        // 开始太空战斗
        SpaceBattle.Instance.StartBattle();
        // 加载战斗场景
        SceneManager.LoadScene("_Scenes/Battle");

        Close();
    }

    public void FixedUpdate()
    {
        if(GameData.Instance.RoomPlayers.Count > 0/* && PannelInit == false*/)
        {
            PannelInit = true;
            RecvGetRoomInfo();
        }
    }
    #endregion
}