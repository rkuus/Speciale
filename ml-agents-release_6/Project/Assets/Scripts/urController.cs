using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class urController : MonoBehaviour
{
    public GameObject[] urJoints;
    public float[] startingRotations;

    public tcpHandler tcp;
    public targetHandler target;

    public bool collisionFlag = false;

    private float[] curRotations;
    private float[] curRotLim;
    //public float curAngle = 0.0f;
    //public float otherAngle = 0.0f;

    //private Vector3 lastDifference;
    //private Vector3 currentDifference;
    //private float lastDistance = 0.0f;
    // Start is called before the first frame update

    public float[] allTriggers;

    void Start()
    {
        curRotations = new float[urJoints.Length];
        curRotLim = new float[urJoints.Length];
        //lastDifference = tcp.transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        getAllTriggers();

        if (collisionCheck())
            collisionFlag = true;
    }

    public float[] getRotations()
    {
        
        for (int jointIndex = 0; jointIndex < urJoints.Length;jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            curRotations[jointIndex] = (joint.CurrentPrimaryAxisRotation()) / 180.0f;
            curRotLim[jointIndex] = curRotations[jointIndex]/2.0f;
            if (curRotations[jointIndex] > 1)
            {
                curRotations[jointIndex] -= 2;
            }
            else if (curRotations[jointIndex] < -1)
            {
                curRotations[jointIndex] += 2;
            }
        }
        return curRotations.Concat(curRotLim).ToArray();
    }

    public float[] getVelocities()
    {
        float[] velocities = new float[urJoints.Length];
        Vector3 lastJoint = new Vector3(0, 0, 0);
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            Vector3 curJoint = joint.getCurrentSpeed();

            if (joint.rotAxis[0])
                velocities[jointIndex] = (curJoint.x- lastJoint.x);
            if (joint.rotAxis[1])
                velocities[jointIndex] = (curJoint.y - lastJoint.y);
            if (joint.rotAxis[2])
                velocities[jointIndex] = (curJoint.z - lastJoint.z);

            lastJoint = curJoint;
        }
        return velocities;
    }

    public bool setRotations(float[] rotations)
    {
        //Debug.Log(rotations.Length + " " + urJoints.Length);
        if (rotations.Length != urJoints.Length)
            return false;
        
        for (int jointIndex = 0; jointIndex < urJoints.Length;jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.jointRotation = rotations[jointIndex];
        }

        return true;
    }

    public void setMaxJointAccerlation(float newAcc)
    {
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.updateMaxAccerlation(newAcc);
        }
    }

    public void setMaxSpeed(float newSpeed)
    {
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.updateMaxSpeed(newSpeed);
        }
    }

    public bool moveRobot(float[] input)
    {
        if (input.Length != urJoints.Length)
            return false;

        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.jointRotation = Mathf.Clamp(input[jointIndex],-1.0f,1.0f);
        }

        return true;
    }

    public void forceARotation(float[] rotations)
    {
        if (urJoints.Length == startingRotations.Length)
        {
            for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
            {
                jointController joint = urJoints[jointIndex].GetComponent<jointController>();
                joint.ForceToRotation(rotations[jointIndex]);
            }
        }
    }

    private bool collisionCheck()
    {
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            if (joint.inCollision)
            {
                return true;
            }             
        }
        return false;
    }
    public float[] getAllTriggers()
    {
        List<float> output = new List<float>();

        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();

            float[] jointOutputs = joint.getTriggers();
            for (int i = 0; i < jointOutputs.Length; i++)
                output.Add(jointOutputs[i]);
            

        }
        allTriggers = output.ToArray();
        return allTriggers;
    }
}
