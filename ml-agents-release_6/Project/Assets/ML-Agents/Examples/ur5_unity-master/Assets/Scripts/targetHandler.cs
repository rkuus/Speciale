using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHandler : MonoBehaviour
{
    public float innerDiameter = 1.0f;
    public float outerDiameter = 5.0f;

    public Vector3 targetPos;
    public Vector3 targetForward;
    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.localPosition * 5;
        targetForward = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = transform.localPosition * 5;
        targetForward = transform.forward;

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

        transform.rotation = Random.rotation;
        //Debug.Log("local:" + transform.localPosition * 5);
        //Debug.Log("global:"+ transform.position);
    }
}
