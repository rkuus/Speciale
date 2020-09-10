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

    private float lastDifference = 20.0f;
    private float curDistance = 20.0f;
    private float curAngle = 180.0f;
    private Vector3 currentDifference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        float[] defaultRotations = { 0.0f, -90.0f, 0.0f, -90.0f, 0.0f, 0.0f};
        robotController.forceARotation(defaultRotations);
        targetBall.updateTargetPos();
        lastDifference = Vector3.Magnitude(tcp.TCPpos - targetBall.targetPos);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // UR configuration
        sensor.AddObservation(robotController.getRotations());  // 6
        //Debug.Log(robotController.getRotations()[0]);
        // UR joint velocities
        sensor.AddObservation(robotController.getVelocities()); // 6
        // End-effector position - target position
        sensor.AddObservation(tcp.TCPpos / 20.0f);              // 3
        sensor.AddObservation(targetBall.targetPos / 20.0f);    // 3
        // Corridinate difference between the tcp and target
        currentDifference = tcp.TCPpos - targetBall.targetPos;
        sensor.AddObservation(currentDifference / 20.0f);       // 3
        // Distance to target
        curDistance = Vector3.Magnitude(currentDifference);
        sensor.AddObservation(curDistance/20.0f);               // 1
        // Rotation of TCP
        //sensor.AddObservation(tcp.transform.rotation.normalized);// 4
        // Angle to target
        curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));
        sensor.AddObservation(curAngle / 180.0f);               // 1
        
        //Debug.Log(currentDifference);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log(vectorAction);
        robotController.setRotations(vectorAction);

        if (curDistance < 0.65f && curAngle < 5.0f && !robotController.collisionFlag)
        {
            SetReward(1f);
            EndEpisode();
        }

        float curReward = 0.0f;

        // Distance reward/cost
        float distanceDif = lastDifference - curDistance;

        curReward += 1.0f * distanceDif / curDistance;
        //Debug.Log("Distance reward:" + (0.1f * distance));
        // time cost
        curReward -= 0.0002f;

        //curReward += 0.0001f / curAngle;

        if (robotController.collisionFlag)
        {
            robotController.collisionFlag = false;
            curReward -= 1.0f;
        }
            

        lastDifference = curDistance;
        //Debug.Log("Total reward:" + curReward);
        SetReward(curReward);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
