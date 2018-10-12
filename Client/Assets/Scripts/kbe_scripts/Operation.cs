namespace KBEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class Operation : OperationBase
    {
        public Operation():base()
        {

        }

        public override void onAttached()
        {
            if (this.owner.isPlayer())
            {
                Event.registerIn("reqRoomList", this, "reqRoomList");
                Event.registerIn("reqEnterRoom", this, "reqEnterRoom");
                Event.registerIn("reqGameBegin", this, "reqGameBegin");
                Event.registerIn("reqLeaveRoom", this, "reqLeaveRoom");
                Event.registerIn("reqCreateRoom", this, "reqCreateRoom");
                Event.registerIn("reqTrueSyncData", this, "reqTrueSyncData");
            }
        }

        public void onDestory()
        {
            if (owner.isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        public override void onLossScoreChanged(Int32 old)
        {
            Debug.Log(owner.className + "::set_lossScore: " + old + " => " + lossScore);
            //Event.fireOut("set_modelID", new object[] { this, this.modelID });
        }

        public override void onWinScoreChanged(Int32 old)
{
            Debug.Log(owner.className + "::set_winScore: " + old + " => " + winScore);
            //Event.fireOut("set_name", new object[] { this, this.name });
        }

        public override void onReqRoomList(ROOM_LIST roomList)
        {
            for(int i = 0;i< roomList.values.Count;i++)
            {
                Debug.Log("Operation::roomkey: " + roomList.values[i].room_key + ",value: " + roomList.values[i].ToString());
            }
            KBEngine.Event.fireOut("onReqRoomList", new object[] { roomList });
        }

        public override void onEnterRoomResult(byte result, ROOM_INFO roomInfo)
        {
            KBEngine.Event.fireOut("onEnterRoomResult", new object[] { result, roomInfo });

            Debug.Log("Operation::onEnterRoomResult: " + "result:"+ result+ ",roomInfo:" + roomInfo.ToString());
        }

        public override void onGameBeginResult(byte result)
        {
            KBEngine.Event.fireOut("onGameBeginResult", new object[] { result});
            Debug.Log("Operation::onGameBeginResult: " + "result:" + result);
        }

        public override void onLeaveRoomResult(byte result)
        {
            KBEngine.Event.fireOut("onLeaveRoomResult", new object[] { result });
            Debug.Log("Operation::onLeaveRoomResult: " + "result:" + result);
        }

        public override void onCreateRoomResult(byte result, ROOM_INFO roomInfo)
        {
            KBEngine.Event.fireOut("onCreateRoomResult", new object[] { result, roomInfo });
            Debug.Log("Operation::onCreateRoomResult: " + "result:" + result);
        }

        public override void onTrueSyncData(byte eventCode, string message)
        {
            KBEngine.Event.fireOut("onTrueSyncData", new object[] {this.owner, eventCode, message });
            Debug.Log("Operation::onTrueSyncData: " + "eventCode:" + eventCode + ",message:"+ message);
        }

        public void reqTrueSyncData(byte eventCode, string message)
        {
            Debug.Log("Operation::reqTrueSyncData:eventCode:"+ eventCode + ",message:"+ message);
            cellEntityCall.reqTrueSyncData(eventCode, message);          
        }

        public void reqRoomList()
        {
            Debug.Log("Operation::reqRoomList" );
            baseEntityCall.reqRoomList();
        }

        public void reqEnterRoom(UInt64 roomKey)
        {
            Debug.Log("Operation::reqEnterRoom: " + "roomKey:" + roomKey);
            baseEntityCall.reqEnterRoom(roomKey);
        }

        public void reqGameBegin()
        {
            Debug.Log("Operation::reqGameBegin" );
            baseEntityCall.reqGameBegin();
        }
        public void reqCreateRoom()
        {
            Debug.Log("Operation::reqCreateRoom " );
            baseEntityCall.reqCreateRoom();
        }

        public void reqLeaveRoom()
        {
            Debug.Log("Operation::reqLeaveRoom " );
            baseEntityCall.reqLeaveRoom();
        }


    }
}
