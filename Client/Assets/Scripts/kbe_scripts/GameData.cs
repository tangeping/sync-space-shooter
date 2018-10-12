using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : CBSingleton<GameData> {

    public KBEngine.Avatar localPlayer = null;
    public string AccountName;
    public ROOM_INFO CurrentRoom;
    public List<KBEngine.Avatar> RoomPlayers = new List<KBEngine.Avatar>();

    public Int32 RoundTripTime = 0;

}
