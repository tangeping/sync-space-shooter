using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEvent : MonoBehaviour {

    public void Start()
    {
        KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
        KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
    }

    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    public void onEnterWorld(KBEngine.Entity entity)
    {
        Debug.Log("WorldEvent:entity." + entity.id + ",claseName:" + entity.className);

        if(entity.isPlayer())
        {
            if(GameData.Instance.RoomPlayers.Count <= 0)
            {
                GameData.Instance.RoomPlayers.Add((KBEngine.Avatar)entity);
            }
            else
            {
                GameData.Instance.RoomPlayers[0] = (KBEngine.Avatar)entity;
            }
        }
        else if(entity.className == "Avatar")
        {
            if (GameData.Instance.RoomPlayers.Count <= 0)
            {
                GameData.Instance.RoomPlayers.Add(new KBEngine.Avatar());   
            }
            GameData.Instance.RoomPlayers.Add((KBEngine.Avatar)entity);

        }

    }

    public void onLeaveWorld(KBEngine.Entity entity)
    {
        Debug.Log("RoomPanel:entity." + entity.id + ",claseName:" + entity.className);

        if (entity.renderObj == null)
            return;

        UnityEngine.GameObject.Destroy((UnityEngine.GameObject)entity.renderObj);
        entity.renderObj = null;
    }


}
