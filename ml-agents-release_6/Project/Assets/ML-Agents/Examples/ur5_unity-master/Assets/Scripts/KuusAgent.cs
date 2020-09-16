using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class KuusAgent : Agent
{
    public urController robotController;
    public tcpHandler tcp;
    public targetHandler targetBall;

    private float curDistance = 20.0f;
    //private float curAngle = 180.0f;
    private float curAngleForward = 180.0f;

    private int decimalPrecision = 2;


    //private float closestEncounter = 999.9f;
    //private float bestAngle = 180.0f;


    private Vector3 currentDifference;

    //private float lastAngle = 180.0f;
    private float lastAngleForward = 180.0f;
    private float lastDistance = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        float[] defaultRotations = { 0.0f, -90.0f, 0.0f, -90.0f, 0.0f, 0.0f};
        robotController.forceARotation(defaultRotations);
        targetBall.updateTargetPos();

        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        curDistance = Vector3.Magnitude(currentDifference);
        lastDistance = curDistance;
        //closestEncounter = curDistance;
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        lastAngleForward = curAngleForward;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // UR configuration
        sensor.AddObservation(roundList(robotController.getRotations(), decimalPrecision));                          // 6
        //Debug.Log(robotController.getRotations()[0]);
        // UR joint velocities
        sensor.AddObservation(roundList(robotController.getVelocities(), decimalPrecision));                         // 6
        // End-effector position - target position
        sensor.AddObservation(roundV3(tcp.TCPpos / 20.0f, decimalPrecision));                                      // 3
        sensor.AddObservation(roundV3(targetBall.gripPlace / 20.0f, decimalPrecision));                            // 3
        // Corridinate difference between the tcp and target
        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        sensor.AddObservation(roundV3(currentDifference / 20.0f, decimalPrecision));                               // 3
        // Distance to target
        curDistance = Vector3.Magnitude(currentDifference);
        sensor.AddObservation(round(curDistance/20.0f, decimalPrecision));                                       // 1
        // Rotation of TCP
        //sensor.AddObservation(tcp.transform.rotation.normalized);                     // 4
        // Angle to target
        //curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.gripPlace - tcp.TCPpos));
        //sensor.AddObservation(round(curAngle / 180.0f, decimalPrecision));                                       // 1
        sensor.AddObservation(roundV3(tcp.TCPforward, decimalPrecision));
        // Rotation forward from target
        sensor.AddObservation(roundV3(targetBall.targetForward, decimalPrecision));                                // 3
        // Rotation between target and tcp forward rotation.
        sensor.AddObservation(roundV3((targetBall.targetForward - tcp.TCPforward)/20.0f, decimalPrecision));       // 3 
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        sensor.AddObservation(round(curAngleForward / 180.0f, decimalPrecision));                                // 1
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log(vectorAction);
        robotController.setRotations(vectorAction);

        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        curDistance = Vector3.Magnitude(currentDifference);
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);

        if (curDistance < 0.25f && !robotController.collisionFlag && curAngleForward < 10.0f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        float curReward = 0.0f;

        // Distance reward/cost

        //if (curDistance < closestEncounter)
        //{
        //    curReward += 0.1f / (1 + curDistance);
        //    closestEncounter = curDistance;
        //}
        //else
        //    curReward -= 0.0005f;

        curReward += 1.0f * (lastDistance - curDistance);

        //if (curAngleForward < bestAngle)
        //{
        //    curReward += 0.1f / (1 + curAngleForward + curDistance);
        //    bestAngle = curAngleForward;
        //}
        //else
        //    curReward -= 0.0001f;

        curReward += 0.01f * (lastAngleForward - curAngleForward);

        curReward -= 0.001f; // time cost

        if (robotController.collisionFlag) // Collision cost.
        {
            robotController.collisionFlag = false;
            curReward -= 0.1f;
        }

        lastDistance = curDistance;
        lastAngleForward = curAngleForward;

        AddReward(curReward);
    }
    // Update is called once per frame
    void Update()
    {
        
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
