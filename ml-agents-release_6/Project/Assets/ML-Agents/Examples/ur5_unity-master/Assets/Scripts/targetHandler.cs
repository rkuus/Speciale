using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHandler : MonoBehaviour
{
    public float innerDiameter = 1.0f;
    public float outerDiameter = 5.0f;

    public GameObject grip;

    public Vector3 targetPos;
    public Vector3 targetForward;
    public Vector3 gripPlace;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.localPosition * 5;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.75f * targetForward;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    updateTargetPos();
        //}
    }

    public void updateTargetPos()
    {
        Vector3 newPos;
        do
        {
            newPos = Random.onUnitSphere * (Random.value * (outerDiameter-innerDiameter) + innerDiameter);
            newPos += new Vector3(0,5,0);
        } while (newPos.y < 5 || Mathf.Abs(newPos.z) < 2.5f || Mathf.Abs(newPos.x) < 2.5f);

        transform.localPosition = newPos / 5;

        do
        {
            transform.rotation = Random.rotation;
            gripPlace = (transform.localPosition * 5) - 0.5f * transform.forward;
        } while ((Vector3.Magnitude(newPos - new Vector3(0, 5, 0)) - 0.40f) < (Vector3.Magnitude(gripPlace - new Vector3(0, 5, 0))));

        targetPos = transform.localPosition * 5;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.75f * targetForward;

        //Debug.Log("center:" + Vector3.Magnitude(newPos - new Vector3(0, 5, 0)));
        //Debug.Log("grip:" + Vector3.Magnitude(gripPlace - new Vector3(0, 5, 0)));
    }
}
