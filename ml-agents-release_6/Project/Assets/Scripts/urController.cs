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
    void Start()
    {
        curRotations = new float[urJoints.Length];
        curRotLim = new float[urJoints.Length];
        //lastDifference = tcp.transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //float translation = Input.GetAxis("Vertical");

        //if (translation > 0)
        //    translation = 1;
        //if (translation < 0)
        //    translation = -1;

        //float[] newRotation = { translation, translation, translation, translation, translation, translation };
        //moveRobot(newRotation);
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    float[] someVels = getVelocities();
        //    Debug.Log("1 vel:" + someVels[0]);
        //    Debug.Log("2 vel:" + someVels[1]);
        //    Debug.Log("3 vel:" + someVels[2]);
        //    Debug.Log("4 vel:" + someVels[3]);
        //    Debug.Log("5 vel:" + someVels[4]);
        //    Debug.Log("6 vel:" + someVels[5]);
        //    //forceARotation(startingRotations);
        //}

        //if (collisionCheck())
        //    Debug.Log("Collision!");

        //Debug.Log("Angle:" + Vector3.Angle(tcp.TCPforward, target.targetForward));
        //Debug.Log(target.targetPos - tcp.TCPpos);

        if (collisionCheck())
            collisionFlag = true;
    }

    public float[] getRotations()
    {
        
        for (int jointIndex = 0; jointIndex < urJoints.Length;jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            curRotations[jointIndex] = (joint.CurrentPrimaryAxisRotation()) / 180.0f;
            if (curRotations[jointIndex] > 1)
            {
                curRotations[jointIndex]--;
                curRotLim[jointIndex] = 1;
            }
            else if (curRotations[jointIndex] < -1)
            {
                curRotations[jointIndex]++;
                curRotLim[jointIndex] = -1;
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
}
