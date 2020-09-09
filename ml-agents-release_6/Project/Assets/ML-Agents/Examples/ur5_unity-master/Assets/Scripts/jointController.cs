using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jointController : MonoBehaviour
{
    public float jointRotation = 0.0f;
    public float speed = 100.0f;
    public bool inCollision = false;
    private int collisionCheck = 0;
    private ArticulationBody articulation;

    // Start is called before the first frame update
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void Update()
    {
        //float translation = Input.GetAxis("Vertical");
        //ArticulationReducedSpace newVelocity = new ArticulationReducedSpace(translation);
        if (jointRotation != 0.0f)
        {
            float rotationChange = (float)jointRotation * speed * Time.fixedDeltaTime;
            float rotationGoal = CurrentPrimaryAxisRotation() + rotationChange;
            RotateTo(rotationGoal);
        }

        if (collisionCheck > 0)
            inCollision = true;
        else
            inCollision = false;

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

    public float getCurrentSpeed()
    {
        return articulation.angularVelocity.y;
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
