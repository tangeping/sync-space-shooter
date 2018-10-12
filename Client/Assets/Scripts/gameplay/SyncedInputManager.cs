using TrueSync;
using UnityEngine;

/// <summary>
/// 输入管理
/// </summary>
public class SyncedInputManager : MonoBehaviour
{
    // 移动增量
    private Vector2 _deltaMovement;

    // 转向增量
    private Vector2 _deltaRotation;

    public Vector2 DeltaMovement
    {
        get { return _deltaMovement; }

        set { _deltaMovement = value; }
    }

    public Vector2 DeltaRotation
    {
        get { return _deltaRotation; }

        set { _deltaRotation = value; }
    }

    /// <summary>
    /// 处理左摇杆移动事件
    /// </summary>
    public void LeftJoystickMovement(Vector2 movement)
    {
        //Debug.Log("left joystick :" + movement);
        if (SpaceBattle.Instance.isBattleStart)
        {
            _deltaMovement = movement;
        }
    }

    /// <summary>
    /// 处理右摇杆移动事件
    /// </summary>
    public void RightJoystickMovement(Vector2 movement)
    {
        //Debug.Log("right joystick :" + movement);
        if (SpaceBattle.Instance.isBattleStart)
        {
            _deltaRotation = movement;
        }
    }
}