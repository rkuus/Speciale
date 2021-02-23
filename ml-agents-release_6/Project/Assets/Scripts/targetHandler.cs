using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHandler : MonoBehaviour
{
    public float outerDiameter = 1.2f;

    public GameObject grip;
    public GameObject ground;
    public GameObject scene;
    public GameObject safetyZone;

    public Vector3 targetPos;
    public Vector3 targetForward;
    public Vector3 gripPlace;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position - ground.transform.position; // transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.15f * targetForward;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            updateTargetPos();
        }
        //targetPos = transform.localPosition;
        //targetForward = transform.forward;
        //gripPlace = targetPos - 0.10f * targetForward;
    }

    public void updateTargetPos()
    {
        Vector3 newPos;
        //safetyZone.SetActive(true);
        //ground.SetActive(false);
        LayerMask mask = ~LayerMask.GetMask("floor");
        do
        {
            newPos = Random.insideUnitSphere * outerDiameter;
            //newPos += new Vector3(0, 0.05f, 0);
        } while (newPos.y < 0 || Physics.CheckSphere(newPos + scene.transform.position, 0.30f, mask)); ;

        //ground.SetActive(true);

        do
        {
            transform.rotation = Random.rotation;
            gripPlace = (newPos) - 0.15f * transform.forward;
        } while ((Vector3.Magnitude(newPos - new Vector3(0, 1.0f, 0)) - 0.05f) < (Vector3.Magnitude(gripPlace - new Vector3(0, 1.0f, 0))) || Physics.CheckSphere(gripPlace + scene.transform.position, 0.10f)); // 

        transform.localPosition = newPos;

        targetPos = transform.position - scene.transform.position; //transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.15f * targetForward;

        //safetyZone.SetActive(false);
        
        //Debug.Log("center:" + Vector3.Magnitude(newPos - new Vector3(0, 1.0f, 0)));
        //Debug.Log("grip:" + Vector3.Magnitude(gripPlace - new Vector3(0, 1.0f, 0)));
    }

    public void updataTargetParams()
    {
        targetPos = transform.position - scene.transform.position; //transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.15f * targetForward;
    }
}
