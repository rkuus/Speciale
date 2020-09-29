using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class urController : MonoBehaviour
{
    public float[] speed;
    public GameObject[] urJoints;
    public float[] startingRotations;

    public tcpHandler tcp;
    public targetHandler target;

    public bool collisionFlag = false;

    //private Vector3 lastDifference;
    //private Vector3 currentDifference;
    //private float lastDistance = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        forceSpeed(speed);
        //lastDifference = tcp.transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //float translation = Input.GetAxis("Vertical");
        //float[] newRotation = { translation, translation, translation, translation, translation, translation };
        //moveRobot(newRotation);
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    forceARotation(startingRotations);
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
        float[] rotations = new float[urJoints.Length];
        for (int jointIndex = 0; jointIndex < urJoints.Length;jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            rotations[jointIndex] = (joint.CurrentPrimaryAxisRotation()) / 360.0f; 
        }
        return rotations;
    }

    public float[] getVelocities()
    {
        float[] velocities = new float[urJoints.Length];
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            velocities[jointIndex] = joint.getCurrentSpeed();
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

    public void forceSpeed(float[] newSpeed)
    {
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.speed = newSpeed[jointIndex];
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
