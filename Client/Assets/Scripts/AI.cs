using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour 
{
    //所控制的坦克
    public Tank tank;

    //状态
    public enum Status
    {
        Patrol,
        Attack,
    }
    private Status status = Status.Patrol;

    //更改状态
    public void ChangeStatus(Status status)
    {
        if (status == Status.Patrol)
            PatrolStart();
        else if (status == Status.Attack)
            AttackStart();
    }

	//状态处理
	void Update () 
    {
        if (tank.ctrlType != Tank.CtrlType.computer)
            return;

        TargetUpdate();
        //行走
        if (_tankPath.IsReach(transform))
        {
            _tankPath.NextWaypoint();
        }

        if (status == Status.Patrol)
            PatrolUpdate();
        else if (status == Status.Attack)
            AttackUpdate();

        
	}


    //巡逻开始
    void PatrolStart()
    {

    }


    //攻击开始
    void AttackStart()
    {
        Vector3 targetPos = target.transform.position;
        _tankPath.InitByNavMeshPath(transform.position, targetPos);
    }


    //巡逻中
    void PatrolUpdate()
    {
        //发现敌人
        if (target != null)
            ChangeStatus(Status.Attack);
        //更新巡逻点
        float interval = Time.time - lastUpdateWaypointTime;
        if (interval < updateWaypointtInterval)
            return;
        lastUpdateWaypointTime = Time.time;

        if (_tankPath.waypoints == null || _tankPath.isFinish)
        {
            GameObject obj = GameObject.Find("WaypointContainer");
            {
                int count = obj.transform.childCount;
                if (count == 0) return;
                int index = Random.Range(0, count);
                Vector3 targetPos = obj.transform.GetChild(index).position;
                _tankPath.InitByNavMeshPath(transform.position, targetPos);
            }
        }
    }

    //攻击中
    void AttackUpdate()
    {
        //目标丢失
        if (target == null)
            ChangeStatus(Status.Patrol);
        //更新巡逻点
        float interval = Time.time - lastUpdateWaypointTime;
        if (interval < updateWaypointtInterval)
            return;
        lastUpdateWaypointTime = Time.time;

        Vector3 targetPos = target.transform.position;
        _tankPath.InitByNavMeshPath(transform.position, targetPos);
    }

    void Start()
    {
        InitWaypoint();
    }

    void OnDrawGizmos()
    {
        _tankPath.DrawWaypoints();
    }

    //----------------搜寻目标----------------------
    //锁定的坦克
    private GameObject target;
    //视野范围
    private float sightDistance = 30;
    //上一次搜寻时间
    private float lastSearchTargetTime = 0;
    //搜寻间隔
    private float searchTargetInterval = 3;

    //搜寻目标
    void TargetUpdate()
    {
        //cd时间
        float interval = Time.time - lastSearchTargetTime;
        if (interval < searchTargetInterval)
            return;
        lastSearchTargetTime = Time.time;

        //已有目标的情况，判断是否丢失目标
        if (target != null)
            HasTarget();
        else
            NoTarget();
    }

    //已有目标的情况，判断是否丢失目标
    void HasTarget()
    {
        Tank targetTank = target.GetComponent<Tank>();
        Vector3 pos = transform.position;
        Vector3 targetPos = target.transform.position;

        if (targetTank.ctrlType == Tank.CtrlType.none)
        {
            Debug.Log("目标死亡，丢失目标");
            target = null;
        }
        else if (Vector3.Distance(pos, targetPos) > sightDistance)
        {
            Debug.Log("距离过远，丢失目标");
            target = null;
        }
    }


    //没有目标的情况，搜索视野中的坦克
    void NoTarget()
    {
        //最小生命值
        float minHp = float.MaxValue; 
        //遍历
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < targets.Length; i++)
        {
            //Tank组件
            Tank tank = targets[i].GetComponent<Tank>();
            if (tank == null)
                continue;
            //自身
            if (targets[i] == gameObject)
                continue;
            //队友
            if (Battle.instance.IsSameCamp(gameObject, targets[i]))
                continue;
            //死亡
            if (tank.ctrlType == Tank.CtrlType.none)
                continue;
            //判断距离
            Vector3 pos = transform.position;
            Vector3 targetPos = targets[i].transform.position;
            if (Vector3.Distance(pos, targetPos) > sightDistance)
                continue;
            //判断生命值
            if (minHp > tank.hp)
                target = tank.gameObject;
        }
        //调试
        if(target != null)
            Debug.Log("获取目标 " + target.name);
    }

    //被攻击
    public void OnAttecked(GameObject attackTank)
    {
        //队友
        if (Battle.instance.IsSameCamp(gameObject, attackTank))
            return;
        target = attackTank;
    }
    //----------------炮塔状态机----------------------

    //获取炮管和炮塔的目标角度
    public Vector3 GetTurretTarget()
    {
        //没有目标，朝着炮塔坦克前方
        if (target == null)
        {
            float y = transform.eulerAngles.y;
            Vector3 rot = new Vector3(0, y, 0);
            return rot;
        }
        //有目标，对准目标
        else
        {
            Vector3 pos = transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 vec = targetPos - pos;
            return Quaternion.LookRotation(vec).eulerAngles;
        }
    }

    //是否发射炮弹
    public bool IsShoot()
    {
        if (target == null)
            return false;

        //获取目标角度差
        float turretRoll = tank.turret.eulerAngles.y;
        float angle = turretRoll - GetTurretTarget().y;
        if (angle < 0) angle += 360;
        //30度以内发射炮弹
        if (angle < 30 || angle > 330)
            return true;
        else
            return false;
    }

    //----------------行走状态机----------------------

    //路径
    private TankPath _tankPath = new TankPath();
    //上次更新路径时间
    private float lastUpdateWaypointTime = float.MinValue;
    //更新路径cd
    private float updateWaypointtInterval = 10;


    //初始化路点
    void InitWaypoint()
    {
        GameObject obj = GameObject.Find("WaypointContainer");
        if (obj && obj.transform.GetChild(0) != null)
        {
            Vector3 targetPos = obj.transform.GetChild(0).position;
            _tankPath.InitByNavMeshPath(transform.position, targetPos);
        }
    }

    //获取转向角
    public float GetSteering()
    {
        if (tank == null)
            return 0;

        Vector3 itp = transform.InverseTransformPoint(_tankPath.waypoint);
        if (itp.x > _tankPath.deviation / 5)
            return tank.maxSteeringAngle;
        else if (itp.x < -_tankPath.deviation / 5)
            return -tank.maxSteeringAngle;
        else
            return 0;
    }

    //获取马力
    public float GetMotor()
    {

        if (tank == null)
            return 0;

        Vector3 itp = transform.InverseTransformPoint(_tankPath.waypoint);
        float x = itp.x;
        float z = itp.z;
        float r = 6;

        if (z < 0 && Mathf.Abs(x) < -z && Mathf.Abs(x) < r)
            return -tank.maxMotorTorque;
        else
            return tank.maxMotorTorque;
    }

    //获取刹车
    public float GetBrakeTorque()
    {
        if (_tankPath.isFinish)
            return tank.maxMotorTorque;
        else
            return 0;
    }

}




