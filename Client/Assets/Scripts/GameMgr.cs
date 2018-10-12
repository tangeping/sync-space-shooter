using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏管理
/// </summary>
public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    // 当前玩家 id
    public string id = "_UnknowPlayerID_";

    void Awake()
    {
        instance = this;
    }
}