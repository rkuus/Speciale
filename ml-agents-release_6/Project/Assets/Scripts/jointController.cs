using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jointController : MonoBehaviour
{
    public float jointRotation = 0.0f;
    private float curJointRotation = 0.0f;
    private float deltaJointRotation = 0.0f;
    public float speed = 100.0f;
    public bool inCollision = false;
    //public bool displayVel = false;
    public bool[] rotAxis = new bool[] { false, false, false };
    private int collisionCheck = 0;
    private ArticulationBody articulation;
    private float maxAcceleration;

    // Start is called before the first frame update
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
        maxAcceleration = 8.0f / ((speed * 0.01f) * Mathf.Rad2Deg);
    }

    // Update is called once per frame
    void Update()
    {
        deltaJointRotation = Mathf.Clamp(jointRotation - curJointRotation, -1f * maxAcceleration, maxAcceleration);
        curJointRotation += deltaJointRotation;

        if (curJointRotation != 0.0f)
        {
            float rotationChange = (float)curJointRotation * speed * Time.fixedDeltaTime; // Mathf.Clamp((float)jointRotation * speed * Time.fixedDeltaTime,-1f * articulation.maxAngularVelocity, 1f * articulation.maxAngularVelocity);
            float rotationGoal = CurrentPrimaryAxisRotation() + rotationChange;
            RotateTo(rotationGoal);
        }

        if (collisionCheck > 0)
            inCollision = true;
        else
            inCollision = false;
        //if (displayVel)
        //{
        //    Debug.Log(maxAcceleration);
        //    Debug.Log(deltaJointRotation);
        //    Debug.Log(articulation.angularVelocity);
        //}
            
    }

    public void ForceToRotation(float rotation)
    {
        // set target
        RotateTo(rotation);

        // force position
        float rotationRads = Mathf.Deg2Rad * rotation;
        ArticulationReducedSpace newPosition = new ArticulationReducedSpace(rotationRads);
        articulation.jointPosition = newPosition;

        // force velocity to zero
        ArticulationReducedSpace newVelocity = new ArticulationReducedSpace(0.0f);
        articulation.jointVelocity = newVelocity;

    }

    public float CurrentPrimaryAxisRotation()
    {
        float currentRotationRads = articulation.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * currentRotationRads; //Mathf.Rad2Deg * currentRotationRads;
        return currentRotation;
    }

    public Vector3 getCurrentSpeed()
    {
        return articulation.angularVelocity;
    }
    void RotateTo(float primaryAxisRotation)
    {
        var drive = articulation.xDrive;
        drive.target = primaryAxisRotation;
        articulation.xDrive = drive;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionCheck++;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionCheck--;
    }
}
