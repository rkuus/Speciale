using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jointController : MonoBehaviour
{
    public float jointRotation = 0.0f;
    private float curJointRotation = 0.0f;
    private float deltaJointRotation = 0.0f;
    public float speed = 100.0f;
    private float accScale = 8.0f;
    //public bool inCollision = false;
    public bool isColliding = false;
    //public bool displayVel = false;
    private int collisionCheck = 0;
    //private int triggerCheck = 0;
    private ArticulationBody articulation;

    private CapsuleCollider capsule;

    private float maxAcceleration;

    private float distance = 0.50f;
    public GameObject aBox;
    public GameObject ground;

    public bool showRays = false;
    public bool[] newRayCasting = { true, true, true, true, true, true, true, true,true,true ,  true, true, true, true, true, true, true, true,true,true ,  true, true, true, true, true, true, true, true,true,true , true, true, true ,true};

    private RaycastHit hit;

    private SphereCollider proximityZone;

    private List<Collider> proximityList = new List<Collider>();

    public Vector3[] proximityOut;
    public bool[] proximityBool = { true, true, true };

    // Start is called before the first frame update
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
        capsule = GetComponent<CapsuleCollider>();
        proximityZone = GetComponent<SphereCollider>();
        maxAcceleration = accScale / ((300 * 0.01f) * Mathf.Rad2Deg);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        deltaJointRotation = Mathf.Clamp(jointRotation - curJointRotation, -1f * maxAcceleration, maxAcceleration);
        curJointRotation += deltaJointRotation;

        if (curJointRotation != 0.0f)
        {
            float rotationChange = (float)curJointRotation * speed * Time.fixedDeltaTime; // Mathf.Clamp((float)jointRotation * speed * Time.fixedDeltaTime,-1f * articulation.maxAngularVelocity, 1f * articulation.maxAngularVelocity);
            float rotationGoal = CurrentPrimaryAxisRotation() + rotationChange;
            RotateTo(rotationGoal);
        }

        //proximityOut = proximityResponse();

    }

    public bool inCollison()
    {
        if (collisionCheck > 0)
        {
            isColliding = true;
            return true;
        }
            
        else
        {
            isColliding = false;
            return false;
        }
            
    }
    public void updateMaxAccerlation(float newAccer)
    {
        accScale = newAccer;
        maxAcceleration = accScale / ((speed * 0.01f) * Mathf.Rad2Deg);
    }

    public void updateMaxSpeed(float newSpeedScale)
    {
        speed = speed * newSpeedScale;
        maxAcceleration = accScale / ((speed * 0.01f) * Mathf.Rad2Deg);
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
        Debug.Log(this.name + " " + collision.collider.name);
        collisionCheck++;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionCheck--;
    }

    public float getJointVelocity()
    {
        return curJointRotation;
    }

    public float[] getTriggers()
    {
        List<float> outputs = new List<float>();

        Vector3 p3 = articulation.worldCenterOfMass + articulation.transform.up * capsule.height * 0.05f;

        Vector3 p4 = p3 + articulation.transform.up * -capsule.height * 0.15f;
        Vector3 p2 = p4 + articulation.transform.up * capsule.height * 0.3f;

        Vector3 p5 = p3 + articulation.transform.up * -capsule.height * 0.5f;
        Vector3 p1 = p5 + articulation.transform.up * capsule.height * 0.95f;

        Vector3 direction = Vector3.RotateTowards(articulation.transform.forward, articulation.transform.right, 0.78539816339f, 0.0f);

        if (newRayCasting[0])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[1])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[2])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = Vector3.RotateTowards(-articulation.transform.forward, articulation.transform.right, 0.78539816339f, 0.0f);

        if (newRayCasting[3])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[4])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[5])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = Vector3.RotateTowards(-articulation.transform.forward, -articulation.transform.right, 0.78539816339f, 0.0f);

        if (newRayCasting[6])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[7])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[8])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = Vector3.RotateTowards(articulation.transform.forward, -articulation.transform.right, 0.78539816339f, 0.0f);

        if (newRayCasting[9])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[10])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[11])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = articulation.transform.right;

        if (newRayCasting[12])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[13])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[14])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = -articulation.transform.right;

        if (newRayCasting[15])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[16])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[17])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = articulation.transform.forward;

        if (newRayCasting[18])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[19])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[20])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        direction = -articulation.transform.forward;

        if (newRayCasting[21])
        {
            if (Physics.CapsuleCast(p1, p3, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[22])
        {
            if (Physics.CapsuleCast(p3, p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p3, direction * (distance), Color.green);
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[23])
        {
            if (Physics.CapsuleCast(p2, p4, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (hit.distance), Color.red);
                    Debug.DrawRay(p4, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p2, direction * (distance), Color.green);
                    Debug.DrawRay(p4, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[24])
        {
            direction = Vector3.RotateTowards(articulation.transform.forward, articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p1, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[25])
        {
            direction = Vector3.RotateTowards(-articulation.transform.forward, articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p1, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[26])
        {
            direction = Vector3.RotateTowards(articulation.transform.right, articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p1, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[27])
        {
            direction = Vector3.RotateTowards(-articulation.transform.right, articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p1, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[28])
        {
            direction = Vector3.RotateTowards(articulation.transform.forward, -articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[29])
        {
            direction = Vector3.RotateTowards(-articulation.transform.forward, -articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[30])
        {
            direction = Vector3.RotateTowards(articulation.transform.right, -articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[31])
        {
            direction = Vector3.RotateTowards(-articulation.transform.right, -articulation.transform.up, 0.78539816339f, 0.0f);

            if (Physics.SphereCast(p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[32])
        {
            direction = articulation.transform.up;

            if (Physics.SphereCast(p1, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p1, direction * (distance), Color.green);
                }
            }
        }

        if (newRayCasting[33])
        {
            direction = -articulation.transform.up;

            if (Physics.SphereCast(p5, capsule.radius, direction, out hit, distance))
            {
                outputs.Add((2 * (hit.distance / distance)) - 1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (hit.distance), Color.red);
                }
            }
            else
            {
                outputs.Add(1);
                if (showRays)
                {
                    Debug.DrawRay(p5, direction * (distance), Color.green);
                }
            }
        }

        return outputs.ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9 && !proximityList.Contains(other))
            proximityList.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9 && proximityList.Contains(other))
            proximityList.Remove(other);
    }

    public Vector3[] proximityResponse()
    {
        List<Vector3> outputs = new List<Vector3>();

        Vector3 p;
        Vector3 p2 = articulation.worldCenterOfMass;

        if (proximityBool[0])
        {
            Vector3 output = new Vector3(0, 0, 0);
            float d;
            float cd = 10f;

            foreach (Collider collider in proximityList)
            {
                p = collider.ClosestPointOnBounds(p2);
                d = Vector3.Distance(p, p2);
                if (d < cd && d != 0)
                {
                    cd = d;
                    output = ((p - p2).normalized) / ((cd * 5f) + 1f);
                    
                }
            }
            if (showRays)
                Debug.DrawRay(p2, output.normalized * cd, Color.red);
            outputs.Add(output);
        }

        if (proximityBool[1])
        {
            Vector3 p1 = p2 + articulation.transform.up * capsule.height * 0.4f;
            Vector3 output = new Vector3(0, 0, 0);
            float d;
            float cd = 10f;

            foreach (Collider collider in proximityList)
            {
                p = collider.ClosestPointOnBounds(p1);
                d = Vector3.Distance(p, p1);
                if (d < cd && d != 0)
                {
                    cd = d;
                    output = ((p - p1).normalized) / ((cd * 5f) + 1f);

                }

            }
            if (showRays)
                Debug.DrawRay(p1, output.normalized * cd, Color.red);
            outputs.Add(output);
        }

        if (proximityBool[2])
        {
            Vector3 p3 = p2 - articulation.transform.up * capsule.height * 0.4f;
            Vector3 output = new Vector3(0, 0, 0);
            float d;
            float cd = 10f;

            foreach (Collider collider in proximityList)
            {
                p = collider.ClosestPointOnBounds(p3);
                d = Vector3.Distance(p, p3);
                if (d < cd && d != 0)
                {
                    cd = d;
                    output = ((p - p3).normalized) / ((cd * 5f)+ 1f);

                }
            }
            if (showRays)
                Debug.DrawRay(p3, output.normalized*cd, Color.red);
            outputs.Add(output);
        }

        proximityOut = outputs.ToArray();
        return outputs.ToArray();
    }
}
