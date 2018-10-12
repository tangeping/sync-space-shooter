using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KBEngine;
using System;

public class LoginPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelLayer.Panel;
    }

    public void Start()
    {
        KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
        KBEngine.Event.registerOut("onLoginSuccessfully", this, "onLoginSuccessfully");
    }

    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
    }
    #endregion



    public void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        //Close();
    }

    public void OnLoginClick()
    {
        //用户名密码为空
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "用户名密码不能为空!");
            return;
        }
        //连接服务器

        KBEngine.Event.fireIn("login", idInput.text, pwInput.text, System.Text.Encoding.UTF8.GetBytes("sync_space_shooter"));

        //NetMgr.Instance.srvConn.Send(protocol, OnLoginBack);

    }

    public void onLoginFailed(UInt16 failedcode)
    {
        Debug.Log("LoginPanel::onLoginFailed");
        PanelMgr.instance.OpenPanel<TipPanel>("", "登录失败，请检查用户名密码!");
    }

    public void onLoginSuccessfully(UInt64 rndUUID, Int32 eid, KBEngine.Avatar accountEntity)
    {
        Debug.Log("LoginPanel::onLoginSuccessfully");
        GameData.Instance.localPlayer = accountEntity;
        GameData.Instance.AccountName = idInput.text;

        PanelMgr.instance.OpenPanel<TipPanel>("", "登录成功!");
        //开始游戏
        PanelMgr.instance.OpenPanel<RoomListPanel>("");
        GameMgr.instance.id = idInput.text;
        


        
        Close();
    }
}