using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class urController : MonoBehaviour
{
    public float speed;
    public GameObject[] urJoints;
    public float[] startingRotations;
    // Start is called before the first frame update
    void Start()
    {
        forceSpeed(speed);
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical");
        float[] newRotation = { translation, translation, translation, translation, translation, translation };
        moveRobot(newRotation);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            forceARotation(startingRotations);
        }

        if (collisionCheck())
            Debug.Log("Collision!");
    }

    public float[] getRotations()
    {
        float[] rotations = new float[urJoints.Length];
        for (int jointIndex = 0; jointIndex < urJoints.Length;jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            rotations[jointIndex] = joint.CurrentPrimaryAxisRotation()/180.0f; 
        }
        return rotations;
    }

    public bool setRotations(float[] rotations)
    {
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

    public void forceSpeed(float newSpeed)
    {
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.speed = newSpeed;
        }
    }

    private bool collisionCheck()
    {
        for (int jointIndex = 0; jointIndex < urJoints.Length; jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            if (joint.inCollision)
                return true;
        }
        return false;
    }
}
