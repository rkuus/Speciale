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

    private float curDistance = 20.0f;
    private float curAngle = 180.0f;
    private float curAngleForward = 180.0f;

    private int decimalPrecision = 3;

    private float[] curRotations;

    //private float closestEncounter = 999.9f;
    //private float bestAngle = 180.0f;


    private Vector3 currentDifference;

    private float lastAngle = 180.0f;
    private float lastAngleForward = 180.0f;
    private float lastDistance = 20.0f;
    //private float bestAngle = 180.0f;
    //private float bestAngleForward = 180.0f;
    //private float bestDistance = 20.0f;

    private bool completed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        if (!completed)
        {
            float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            robotController.forceARotation(defaultRotations);
        }
        completed = false;
        //collisionCost += 0.01f;

        for (int i = 0; i < allObs.Length; i++)
            allObs[i].updateObsPos();

        targetBall.updateTargetPos();

        currentDifference = tcp.TCPpos - targetBall.gripPlace;
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

        // UR configuration
        curRotations = robotController.getRotations();
        sensor.AddObservation(roundList(curRotations, decimalPrecision));                                  // 12
        // UR joint velocities
        sensor.AddObservation(roundList(robotController.getVelocities(), decimalPrecision));               // 6
        // End-effector position - target position
        sensor.AddObservation(roundV3(tcp.TCPpos, decimalPrecision));                                      // 3
        sensor.AddObservation(roundV3(targetBall.gripPlace, decimalPrecision));                            // 3
        // Corridinate difference between the tcp and target
        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        sensor.AddObservation(roundV3(currentDifference, decimalPrecision));                               // 3
        // Distance to target
        curDistance = Vector3.SqrMagnitude(currentDifference);
        sensor.AddObservation(round(curDistance, decimalPrecision));                                       // 1
        // Rotation of TCP
        //sensor.AddObservation(tcp.transform.rotation.normalized);                                        // 4
        // Angle to target
        curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));
        sensor.AddObservation(round(curAngle / 180.0f, decimalPrecision));                                // 1
        sensor.AddObservation(roundV3(tcp.TCPforward, decimalPrecision));
        // Rotation forward from target
        sensor.AddObservation(roundV3(targetBall.targetForward, decimalPrecision));                         // 3
        // Rotation between target and tcp forward rotation.
        sensor.AddObservation(roundV3((targetBall.targetForward - tcp.TCPforward), decimalPrecision));      // 3 
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        sensor.AddObservation(round(curAngleForward / 180.0f, decimalPrecision));                           // 1
        // Voxel grid
        //sensor.AddObservation(voxelGrid.voxelCollisions());                                                 // 45
        //curAngle = Vector3.Angle(tcp.TCPpos, targetBall.targetPos);
        //sensor.AddObservation(round(curAngle / 180.0f, decimalPrecision));                                  // 1
        // 3D sensors
        //sensor.AddObservation(roundList(firstSensor.getSensorData(), decimalPrecision));                    // 90
        //sensor.AddObservation(roundList(secondSensor.getSensorData(), decimalPrecision));                   // 90
        //sensor.AddObservation(roundList(depthThing.getRayCasts(), decimalPrecision));
        sensor.AddObservation(roundList(robotController.getAllTriggers(), decimalPrecision));
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log(vectorAction);
        robotController.setRotations(roundList(vectorAction,1));
        CalcReward();
    }
    // Update is called once per frame
    void Update()
    {
        if (robotController.collisionFlag) // Collision cost.
        {
            robotController.collisionFlag = false;
            AddReward(-0.5f);
        }
    }

    private void CalcReward()
    {
        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        curDistance = Vector3.SqrMagnitude(currentDifference);
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));

        float curReward = -0.0005f; // Time cost

        curReward += 5.0f * (lastDistance - curDistance); // reward for approaching

        curReward += 0.05f * (lastAngleForward - curAngleForward); // reward for correct angle
            
        curReward += 0.05f * (lastAngle - curAngle);

            
        //for (int i = 6; i < curRotations.Length; i++)
        //    if (Mathf.Abs(curRotations[i]) > 0.5f)
        //        curReward -= 0.01f;

        //curReward -= 0.0001f * squaredList(vectorAction); // squared sum of actions, Smoothness

        if (curDistance < 0.15f && curAngleForward < 15.0f && curAngle < 15.0f)
        {
            curReward += 0.5f;
            AddReward(curReward);
            completed = true;
            EndEpisode();
        }

        lastAngle = curAngle;
        lastAngleForward = curAngleForward;
        lastDistance = curDistance;

        AddReward(curReward);
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
