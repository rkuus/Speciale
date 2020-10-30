using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using System.Linq;

public class aiProjectAgent : Agent
{
    public urController robotController;
    public tcpHandler tcp;
    public targetHandler targetBall;

    private float curDistance = 20.0f;

    private int decimalPrecision = 3;

    private float lastDistance = 1.0f;

    private float[] lastestRayCast;

    private int lookingCounter = 0;
    //private float[] recurrentValue = { 0.0f,0.0f,0.0f,0.0f };

    private List<Vector2> exploration = new List<Vector2>();

    private float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    public override void OnEpisodeBegin()
    {
        robotController.forceARotation(defaultRotations);

        targetBall.updateTargetPos();

        curDistance = 1.0f;
        lastDistance = curDistance;

        lookingCounter = 0;

        exploration.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // UR configuration
        sensor.AddObservation(roundList(robotController.getRotations(), decimalPrecision));                // 6
        // UR joint velocities
        sensor.AddObservation(roundList(robotController.getVelocities(), decimalPrecision));               // 6
        // End-effector position
        sensor.AddObservation(roundV3(tcp.TCPpos / 2.0f, decimalPrecision));                               // 3
        // End-effector direction
        sensor.AddObservation(roundV3(tcp.TCPforward, decimalPrecision));                                  // 3 
        // RayCast
        lastestRayCast = getRayCasts();
        sensor.AddObservation(roundList(lastestRayCast, decimalPrecision));                                // 10
        // Collision
        sensor.AddObservation(robotController.collisionFlag);                                              // 1
        // Time
        sensor.AddObservation(round((float)StepCount / (float)MaxStep,decimalPrecision));                  // 1
        //// Memory test
        //sensor.AddObservation(recurrentValue);                                                             // 4
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //float[] robotInput = vectorAction.Take(6).ToArray();
        robotController.setRotations(vectorAction);

        //recurrentValue = vectorAction.Skip(6).ToArray();

        float curReward = -0.002f; // time cost

        if (robotController.collisionFlag)
        {
            robotController.collisionFlag = false;
            curReward -= 0.1f; // Collision cost

            curReward -= 0.1f * lookingCounter;
            lookingCounter = 0;
        }
        else
        {
            if (explored())
                curReward += 0.01f;

            if (lastestRayCast[1] == 1.0f)
            {
                lastDistance = curDistance;
                curDistance = lastestRayCast[0];
                curReward += 1.0f * (lastDistance - curDistance);

                if (curDistance < 0.20f)
                {
                    lookingCounter++;
                    curReward += 0.1f;
                    if (lookingCounter > 99)
                    {
                        curReward += 1.0f;
                        AddReward(curReward);
                        EndEpisode();
                    }
                }
                else
                {
                    curReward -= 0.1f * lookingCounter;
                    lookingCounter = 0;
                }
                    
            }
        }

        AddReward(curReward);
    }

    private bool explored()
    {
        Vector3 explored = tcp.TCPpos + (tcp.TCPforward * lastestRayCast[0] * 1.5f);
        Vector2 exploredv2 = new Vector2(explored.x, explored.z);

        if (explored.y > 0.45f || exploredv2.magnitude > 2.25f)
            return false;


        for (int i = 0;i< exploration.Count;i++)
        {
            if (Vector2.Distance(exploration[i], exploredv2) < 0.1f)
                return false;
        }
        exploration.Add(exploredv2);
        return true;
    }
    private float[] getRayCasts()
    {
        float[] rayHits = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

        Vector3[] directions = { tcp.transform.right, tcp.transform.up, -1.0f * tcp.transform.right, -1.0f * tcp.transform.up };
        Vector3[] nextDirections = directions;

        RaycastHit hit;
        Vector3 thisDirection;

        if (Physics.SphereCast(tcp.TCPpos + transform.position,0.045f, tcp.transform.forward, out hit, 1.5f))
        {
            //Debug.DrawRay(tcp.TCPpos + transform.position, tcp.transform.forward * hit.distance, Color.red);
            rayHits[0] = hit.distance / 1.5f;
            if (hit.collider.ToString() == "Target (UnityEngine.SphereCollider)")
            {
                rayHits[1] = 1.0f;
            }
        }
        else
        {
            //Debug.DrawRay(tcp.TCPpos + transform.position, tcp.transform.forward * 1.5f, Color.yellow);
            rayHits[0] = 1.0f;
        }
        
        for (int i = 0; i < 4; i++)
        {

            thisDirection = Vector3.RotateTowards(tcp.transform.forward, directions[i], 0.25f, 0.0f);
            nextDirections[i] = thisDirection;

            if (Physics.SphereCast(tcp.TCPpos + transform.position, 0.045f, thisDirection, out hit, 1.5f))
            {
                //Debug.DrawRay(tcp.TCPpos + transform.position, thisDirection * hit.distance, Color.red);
                rayHits[i+2] = hit.distance / 1.5f;
            }
            else
            {
                //Debug.DrawRay(tcp.TCPpos + transform.position, thisDirection * 1.5f, Color.yellow);
                rayHits[i + 2] = 1.0f;
            }
        }

        float angle = Vector3.Angle(nextDirections[3], nextDirections[0]) * Mathf.Deg2Rad;

        thisDirection = Vector3.RotateTowards(nextDirections[3], nextDirections[0], angle * 0.5f, 0.0f);

        if (Physics.SphereCast(tcp.TCPpos + transform.position, 0.045f, thisDirection, out hit, 1.5f))
        {
            rayHits[6] = hit.distance / 1.5f;
            //Debug.DrawRay(tcp.TCPpos + transform.position, thisDirection * hit.distance, Color.red);
        }
        else
        {
            rayHits[6] = 1.0f;
            //Debug.DrawRay(tcp.TCPpos + transform.position, thisDirection * 1.5f, Color.yellow);
        }

        for (int i = 0; i < 3; i++)
        {

            thisDirection = Vector3.RotateTowards(nextDirections[i], nextDirections[i+1], 0.25f, 0.0f);

            if (Physics.SphereCast(tcp.TCPpos + transform.position, 0.045f, thisDirection, out hit, 1.5f))
            {
                //Debug.DrawRay(tcp.TCPpos + transform.position, thisDirection * hit.distance, Color.red);
                rayHits[i + 7] = hit.distance / 1.5f;
            }
            else
            {
                rayHits[i + 7] = 1.0f;
                //Debug.DrawRay(tcp.TCPpos + transform.position, thisDirection * 1.5f, Color.yellow);
            }
        }

        return rayHits;
    }
    private static float[] roundList(float[] values, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        float[] newValues = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
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
