using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.IO;

public class KuusAgent : Agent
{
    public urController robotController;
    public tcpHandler tcp;
    public targetHandler targetBall;
    public VoxelGridCreator voxelGrid;
    public sensor3D firstSensor;
    public sensor3D secondSensor;
    public DepthMap depthThing;
    public GameObject safetyZone;

    public Vector3 eulerAngleDif;

    public generalObstacles[] allObs;

    private bool updateAccelerationAndSpeed = true;
    public float maxJointAccelerationScale = 1.0f; // Normal value is 8
    private float maxJointAcceleration = 8.0f;
    public float maxJointSpeedScale = 1.0f; // Normal value is 1

    private float winDistance = 0.10f;
    private float winAngle = 10.0f;
    private float winAngleForward = 10.0f;

    private float decDistance = 0.00005f;
    private float decAngle = 0.002f;
    private float decAngleForward = 0.002f;

    private float stopDistance = 0.10f;
    private float stopAngle = 10.0f;
    private float stopAngleForward = 10.0f;

    private float collisionCost = 0.05f;

    private float curDistance = 20.0f;
    private float curAngle = 180.0f;
    private float curAngleForward = 180.0f;

    //private int decimalPrecision = 4;

    private float[] curRotations;
    private float[] curVelocity;
    private float[] curAction;

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

    //private float episodeReward = 0.0f;

    private bool completed = false;
    private bool jointLimit = false;

    public bool debugMode = false;
    private int debugJointLimit = 0;
    private int debugCollisions = 0;
    private int debugCompleted  = 0;
    private float debugReward   = 0;
    private int debugTimeSteps  = 0;


    private int _time = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (debugMode)
        {
            winDistance = stopDistance;
            winAngle = stopAngle;
            winAngleForward = stopAngleForward;
        }
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
        }
        completed = false;
        jointLimit = false;
        robotController.collisionFlag = false;
        //collisionCost += 0.01f;
        safetyZone.transform.localPosition = new Vector3(0,0.5f,0);

        for (int i = 0; i < allObs.Length; i++)
            allObs[i].updatePos();

        if (updateAccelerationAndSpeed)
        {
            updateAccelerationAndSpeed = false;
            robotController.setMaxSpeed(maxJointSpeedScale);
            robotController.setMaxJointAccerlation(maxJointAcceleration * maxJointAccelerationScale);
        }

        targetBall.updateTargetPos();

        safetyZone.transform.localPosition = new Vector3(0, 10f, 0);

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
        curVelocity = robotController.getVelocities();
        curAction = curVelocity;

        if (debugMode)
        {
            writeToFile();
            debugCollisions = 0;
            debugJointLimit = 0;
            debugReward = 0;
            debugTimeSteps = 0;
            debugCompleted = 0;
            Debug.Log("Episode complete");
        }
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //eulerAngleDif = targetBall.eulerAngles - tcp.eulerAngles;
        //tcp.updateParams();
        //targetBall.updataTargetParams();
        // UR configuration
        //curRotations = robotController.getRotations();
        sensor.AddObservation(curRotations);                                  // 10
        //Debug.Log(curRotations.Length);
        // UR joint velocities
        sensor.AddObservation(curVelocity);               // 5
        // End-effector position - target position
        sensor.AddObservation(tcp.TCPpos / 2f);                                      // 3
        sensor.AddObservation(targetBall.gripPlace / 2f);                            // 3
        // Corridinate difference between the tcp and target
        //currentDifference = tcp.TCPpos - targetBall.gripPlace;
        sensor.AddObservation(currentDifference / 2f );                               // 3
        // NEW INPUTS
        sensor.AddObservation(lastDifference / 2f);
        sensor.AddObservation((lastDifference - currentDifference) / 2f);
        // Distance to target
        sensor.AddObservation(curDistance / 2f);                                       // 1ps 
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
        // 3D sensors
        sensor.AddObservation(robotController.getAllTriggers());
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        CalcReward();
        curAction = vectorAction;
        robotController.setRotations(vectorAction);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        _time ++;

        tcp.updateParams();
        targetBall.updataTargetParams();
        currentDifference = tcp.TCPpos - targetBall.gripPlace;
        curRotations = robotController.getRotations();
        curVelocity = robotController.getVelocities();
        curDistance = Vector3.Magnitude(currentDifference);
        curAngleForward = Vector3.Angle(tcp.TCPforward, targetBall.targetForward);
        curAngle = Vector3.Angle(tcp.TCPforward, (targetBall.targetPos - tcp.TCPpos));

        if (!robotController.collisionFlag && curDistance < winDistance && curAngleForward < winAngleForward && curAngle < winAngle)
        {
            if (debugMode)
                debugCompleted = 1;
            SetReward(1.0f);
            debugReward += 1.0f;
            completed = true;
            EndEpisode();
            
        }
    }

    private void CalcReward()
    {
        float curReward = -0.002f * _time; // Time cost, -0.0001f


        curReward += 4.0f * (lastDistance - curDistance);
  
        curReward += 0.01f * (lastAngleForward - curAngleForward); // reward for aligning with target

        curReward += 0.01f * (lastAngle - curAngle); // reward for facing target


        for (int i = 5;i<curRotations.Length;i++)
        {
            if (Mathf.Abs(curRotations[i]) >= 1f)
            {
                jointLimit = true;
                curReward -= 1f; // collisionCost * _time;
                if (debugMode)
                {
                    Debug.Log("Joint at limit, end episode");
                    debugJointLimit = 1;
                    debugTimeSteps += 1;
                    debugReward += curReward;
                }
                    
                SetReward(curReward);
                EndEpisode();
            }
        }

        if (robotController.collisionFlag) // Collision cost.
        {
            robotController.collisionFlag = false;
            curReward -= collisionCost * _time;
            if (debugMode)
            {
                Debug.Log("Collision");
                debugCollisions += 1;
            }
                
        }

        _time = 0;

        lastAngle = curAngle;
        lastAngleForward = curAngleForward;
        lastDistance = curDistance;

        if (debugMode)
        {
            debugTimeSteps += 1;
            debugReward += curReward;
        }
            

        SetReward(curReward);
    }

    private static float expReward(float x)
    {
        return 1 / (x + 0.2f);
    }
    private static float squaredListDifference(float[] values1, float[] values2)
    {
        float output = 0.0f;
        for (int i = 0;i< values1.Length;i++)
        {
            output += (values1[i] - values2[i])  * (values1[i] - values2[i]);
        }
        return Mathf.Sqrt(output);
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

    private void writeToFile()
    {
        string path = "Assets/Resources/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(debugTimeSteps + ";" + debugReward.ToString("#.000") + ";" + debugCollisions + ";" + debugJointLimit + ";" + curDistance.ToString("#.000") + ";" + curAngle.ToString("#.000") + ";" + curAngleForward.ToString("#.000") + ";" + debugCompleted);
        writer.Close();
    }

}
