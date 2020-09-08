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

    private Vector3 lastDifference;
    private Vector3 currentDifference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        robotController.forceARotation(defaultRotations);
        targetBall.updateTargetPos();
        lastDifference = tcp.transform.position - targetBall.transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // UR configuration
        sensor.AddObservation(robotController.getRotations());
        // UR joint velocities
        sensor.AddObservation(robotController.getVelocities());
        // End-effector position - target position
        currentDifference = tcp.transform.position - targetBall.transform.position;
        sensor.AddObservation(currentDifference);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log(vectorAction);
        robotController.setRotations(vectorAction);

        float magnitudeCurrent = Vector3.Magnitude(currentDifference);
        float distance = Vector3.Magnitude(lastDifference) - magnitudeCurrent;
        if (magnitudeCurrent < 1)
        {
            SetReward(1f);
            EndEpisode();
        }

        float curReward = 0.0f;

        // Distance reward/cost
        curReward += 0.01f * distance;

        // time cost
        curReward -= 0.001f;

        lastDifference = currentDifference;

        SetReward(curReward);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
