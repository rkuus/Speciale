using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class KuusAgent : Agent
{
    public urController robotController;
    public tcpHandler tcp;
    public targetHandler targetBall;
    public VoxelGridCreator voxelGrid;
    public sensor3D firstSensor;
    public sensor3D secondSensor;
    public DepthMap depthThing;

    public obsScript[] allObs;

    private bool updateAccelerationAndSpeed = true;
    public float maxJointAccelerationScale = 1.0f; // Normal value is 8
    private float maxJointAcceleration = 8.0f;
    public float maxJointSpeedScale = 1.0f; // Normal value is 1

    private float winDistance = 0.15f;
    private float winAngle = 25.0f;
    private float winAngleForward = 25.0f;

    private float decDistance = 0.0001f;
    private float decAngle = 0.01f;
    private float decAngleForward = 0.01f;

    private float stopDistance = 0.05f;
    private float stopAngle = 10.0f;
    private float stopAngleForward = 10.0f;

    private float collisionCost = 0.10f;
    private float collisionCostInc = 0.01f;
    private float collisionCostStop = 0.1f;

    private float curDistance = 20.0f;
    private float curAngle = 180.0f;
    private float curAngleForward = 180.0f;

    //private int decimalPrecision = 4;

    private float[] curRotations;

    //private float closestEncounter = 999.9f;
    //private float bestAngle = 180.0f;
    private Vector3 currentDifference;
    private Vector3 lastDifference;

    private float lastAngle = 180.0f;
    private float lastAngleForward = 180.0f;
    private float lastDistance = 20.0f;
    //private float bestAngle = 180.0f;
    //private float bestAngleForward = 180.0f;
    //private float bestDistance = 20.0f;

    private bool completed = false;
    private bool jointLimit = false;
    public bool debugMode = false;

    private int _time = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        if (robotController.collisionFlag)
        {
            float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
            robotController.forceARotation(defaultRotations);
        }
        else if (jointLimit)
        {
            float[] defaultRotations = robotController.getRawRotation();
            for (int i = 0;i<defaultRotations.Length;i++)
                if (Mathf.Abs(defaultRotations[i])>355f)
                {
                    if (defaultRotations[i] < 0)
                        defaultRotations[i] += 360f;
                    else
                        defaultRotations[i] -= 360f;
                }
            robotController.forceARotation(defaultRotations);
        }
        if (completed)
        {
            if (winDistance > stopDistance)
                winDistance -= decDistance;

            if (winAngle > stopAngle)
                winAngle -= decAngle;

            if (winAngleForward > stopAngleForward)
                winAngleForward -= decAngleForward;

            if (collisionCost < collisionCostStop)
                collisionCost += collisionCostInc;
        }
        completed = false;
        jointLimit = false;
        robotController.collisionFlag = false;
        //collisionCost += 0.01f;

        for (int i = 0; i < allObs.Length; i++)
            allObs[i].updateObsPos();

        if (updateAccelerationAndSpeed)
        {
            updateAccelerationAndSpeed = false;
            robotController.setMaxSpeed(maxJointSpeedScale);
            robotController.setMaxJointAccerlation(maxJointAcceleration * maxJointAccelerationScale);
        }

        targetBall.updateTargetPos();
        tcp.updateParams();
        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        lastDifference = currentDifference;
        curDistance = Vector3.Magnitude(currentDifference);
        //lastDistance = curDistance;
        lastDistance = curDistance;
        //closestEncounter = curDistance;
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        //lastAngleForward = curAngleForward;
        lastAngleForward = curAngleForward;

        curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));
        //lastAngle = curAngle;
        lastAngle = curAngle;
        curRotations = robotController.getRotations();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //tcp.updateParams();
        //targetBall.updataTargetParams();
        // UR configuration
        //curRotations = robotController.getRotations();
        sensor.AddObservation(curRotations);                                  // 10
        Debug.Log(curRotations.Length);
        // UR joint velocities
        sensor.AddObservation(robotController.getVelocities());               // 5
        // End-effector position - target position
        sensor.AddObservation(tcp.TCPpos);                                      // 3
        sensor.AddObservation(targetBall.gripPlace);                            // 3
        // Corridinate difference between the tcp and target
        //currentDifference = tcp.TCPpos - targetBall.gripPlace;
        sensor.AddObservation(currentDifference);                               // 3
        // NEW INPUTS
        sensor.AddObservation(lastDifference);
        sensor.AddObservation(lastDifference - currentDifference);
        // Distance to target
        //curDistance = Vector3.SqrMagnitude(currentDifference);
        sensor.AddObservation(curDistance);                                       // 1
        // Rotation of TCP
        //sensor.AddObservation(tcp.transform.rotation.normalized);                                        // 4
        // Angle to target
        //curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));
        sensor.AddObservation(curAngle / 180.0f);                                // 1
        sensor.AddObservation(tcp.TCPforward);
        // Rotation forward from target
        sensor.AddObservation(targetBall.targetForward);                         // 3
        // Rotation between target and tcp forward rotation.
        sensor.AddObservation((targetBall.targetForward - tcp.TCPforward));      // 3 
        //curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        sensor.AddObservation(curAngleForward / 180.0f);                           // 1
        // Voxel grid
        //sensor.AddObservation(voxelGrid.voxelCollisions());                                                 // 45
        //curAngle = Vector3.Angle(tcp.TCPpos, targetBall.targetPos);
        //sensor.AddObservation(round(curAngle / 180.0f, decimalPrecision));                                  // 1
        // 3D sensors
        //sensor.AddObservation(roundList(firstSensor.getSensorData(), decimalPrecision));                    // 90
        //sensor.AddObservation(roundList(secondSensor.getSensorData(), decimalPrecision));                   // 90
        //sensor.AddObservation(roundList(depthThing.getRayCasts(), decimalPrecision));
        sensor.AddObservation(robotController.getAllTriggers());
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log(vectorAction);
        CalcReward();

        //float vectorScale = Mathf.Clamp(curDistance * 5f, 0.35f, 1f); // Game 2 uses scaling 5, and only on 3 joints. Game 3 uses no scaling

        //for (int i = 0; i < 3; i++)
        //    if (Mathf.Abs(vectorAction[i]) < 0.2f)
        //        vectorAction[i] = 0f;
            //else
            //    vectorAction[i] = vectorAction[i] * vectorScale;

        robotController.setRotations(vectorAction);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        _time ++;

        tcp.updateParams();
        targetBall.updataTargetParams();
        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        curDistance = Vector3.SqrMagnitude(currentDifference);
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));

        if (!robotController.collisionFlag && curDistance < winDistance && curAngleForward < winAngleForward && curAngle < winAngle)
        {
            SetReward(1.0f);
            completed = true;
            EndEpisode();
        }
    }

    private void CalcReward()
    {

        float curReward = -0.001f * _time; // Time cost, -0.0001f

        _time = 0;

        curReward += 2.0f * (lastDistance - curDistance); // reward for approaching

        curReward += 0.01f * (lastAngleForward - curAngleForward); // reward for aligning with target

        curReward += 0.01f * (lastAngle - curAngle); // reward for facing target

        for (int i = 5;i<curRotations.Length;i++)
        {
            if (Mathf.Abs(curRotations[i]) >= 0.99f)
            {
                jointLimit = true;
                curReward -= 1f; // collisionCost * _time;
                if (debugMode)
                    Debug.Log("Joint at limit, end episode");
                SetReward(curReward);
                EndEpisode();
            }
        }

        if (robotController.collisionFlag) // Collision cost.
        {
            robotController.collisionFlag = false;
            curReward -= collisionCost * _time;
            if (debugMode)
                Debug.Log("Collision");
        }

        lastAngle = curAngle;
        lastAngleForward = curAngleForward;
        lastDistance = curDistance;
        
        //if (debugMode)
        //    Debug.Log(curReward);

        SetReward(curReward);
    }

    private static float squaredList(float[] values)
    {
        float output = 0.0f;
        for (int i = 0;i<values.Length;i++)
        {
            output += values[i] * values[i];
        }
        return output;
    }
    private static float[] roundList(float[] values, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        float[] newValues = new float[values.Length];
        for (int i = 0;i<values.Length;i++)
        {
            newValues[i] = Mathf.Round(values[i] * mult) / mult;
        }
        return newValues;
    }
    private static float round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
    private static Vector3 roundV3(Vector3 values, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return new Vector3((Mathf.Round(values.x * mult) / mult), (Mathf.Round(values.y * mult) / mult), (Mathf.Round(values.z * mult) / mult));
    }

}
