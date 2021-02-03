using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHandler : MonoBehaviour
{
    public float innerDiameter = 0.4f;
    public float outerDiameter = 1.2f;

    private float innerDSquared = 0.0f;

    public bool fixedY = false;

    public GameObject grip;
    public GameObject ground;

    public Vector3 targetPos;
    public Vector3 targetForward;
    public Vector3 gripPlace;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position - ground.transform.position; // transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.10f * targetForward;
        innerDSquared = innerDiameter * innerDiameter;
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    updateTargetPos();
        //}
        //targetPos = transform.localPosition;
        //targetForward = transform.forward;
        //gripPlace = targetPos - 0.10f * targetForward;
    }

    public void updateTargetPos()
    {
        Vector3 newPos;
        do
        {
            newPos = Random.onUnitSphere * (Random.value * (outerDiameter - innerDiameter) + innerDiameter);
            //newPos += new Vector3(0, 0.05f, 0);
            if (fixedY)
                newPos.y = 0.25f;
        } while (newPos.y < 0.1f || Vector3.Magnitude(new Vector3(newPos.x, 0, newPos.z)) < innerDSquared || Physics.OverlapSphere(newPos + ground.transform.position, 0.40f, ~0, QueryTriggerInteraction.Ignore).Length > 0); ;

        transform.localPosition = newPos;

        do
        {
            transform.rotation = Random.rotation;
            gripPlace = (transform.localPosition) - 0.10f * transform.forward;
        } while ((Vector3.Magnitude(newPos - new Vector3(0, 1.0f, 0)) - 0.025f) < (Vector3.Magnitude(gripPlace - new Vector3(0, 1.0f, 0))));



        targetPos = transform.position - ground.transform.position; //transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.10f * targetForward;


        //Debug.Log("center:" + Vector3.Magnitude(newPos - new Vector3(0, 1.0f, 0)));
        //Debug.Log("grip:" + Vector3.Magnitude(gripPlace - new Vector3(0, 1.0f, 0)));
    }

    public void updataTargetParams()
    {
        targetPos = transform.position - ground.transform.position; //transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.15f * targetForward;
    }
}
