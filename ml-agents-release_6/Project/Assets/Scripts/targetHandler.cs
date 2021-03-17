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
    public Vector3 eulerAngles;

    public bool validationScene = false;
    public Vector3 workzoneLowerCorner;
    public Vector3 workzoneUpperCorner;

    private float gripPlaceOffSet;
    private Quaternion originalRotationValue;
    // Start is called before the first frame update
    void Start()
    {
        originalRotationValue = transform.rotation;
        targetPos = transform.position - ground.transform.position; // transform.localPosition;
        targetForward = transform.forward;
        gripPlaceOffSet = transform.localScale.x * 0.50f;
        gripPlace = targetPos - gripPlaceOffSet * targetForward;
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
        Vector3 checkGrip;
        LayerMask mask = ~LayerMask.GetMask("floor");
        LayerMask mask2 = LayerMask.GetMask("floor", "obstacles");
        if (validationScene)
        {
            
            do
            {
                newPos = new Vector3(Random.Range(workzoneLowerCorner.x, workzoneUpperCorner.x), Random.Range(workzoneLowerCorner.y, workzoneUpperCorner.y), Random.Range(workzoneLowerCorner.z, workzoneUpperCorner.z));
            } while (Vector3.Magnitude(newPos-transform.parent.position) > outerDiameter || Physics.CheckSphere(newPos, 0.20f, mask));

            transform.position = newPos;

            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, 1f);
            if (newPos.y > 0.80f)
                transform.Rotate(new Vector3(1, 0, 0), -90f);
            else if (newPos.y < 0.20f)
                transform.Rotate(new Vector3(1, 0, 0), 90f);
            else
            {
                do
                {

                    transform.rotation = Random.rotation;

                    gripPlace = transform.localPosition - gripPlaceOffSet * transform.forward;

                } while (Vector3.Magnitude(transform.localPosition - new Vector3(0, 0.7f, 0)) < Vector3.Magnitude(gripPlace - new Vector3(0, 0.7f, 0))); ;
            }


            //eulerAngles = transform.rotation.eulerAngles / 360f;
            targetPos = transform.position - scene.transform.position; //transform.localPosition;
            targetForward = transform.forward;
            gripPlace = targetPos - gripPlaceOffSet * targetForward;
            return;
        }

        float oriRadius = safetyZone.GetComponent<CapsuleCollider>().radius;
        safetyZone.GetComponent<CapsuleCollider>().radius = 0.3f;
        bool solutionMissing = true;

        do
        {
            do
            {
                newPos = Random.insideUnitSphere * outerDiameter;
                newPos.y += 0.2f;
            } while (newPos.y < 0 || Physics.CheckSphere(newPos + scene.transform.position, 0.20f, mask)); ;

            for (int i = 0; i < 100; i++)
            {
                transform.rotation = Random.rotation;
                gripPlace = newPos - (gripPlaceOffSet * transform.forward);
                checkGrip = newPos - (3.0f * gripPlaceOffSet * transform.forward);

                if ((Vector3.Magnitude(newPos - new Vector3(0, 0.7f, 0))- gripPlaceOffSet*0.35f) > Vector3.Magnitude(gripPlace - new Vector3(0, 0.7f, 0))
                    && !Physics.CheckCapsule(checkGrip + scene.transform.position, new Vector3(0, 0.7f, 0) + scene.transform.position, 0.15f, mask2))
                {
                    solutionMissing = false;
                    break;
                }
            }
        } while (solutionMissing);

        transform.localPosition = newPos;

        eulerAngles = transform.rotation.eulerAngles / 360f;
        targetPos = transform.position - scene.transform.position; //transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - gripPlaceOffSet * targetForward;

        safetyZone.GetComponent<CapsuleCollider>().radius = oriRadius;
        //Debug.Log("center:" + Vector3.Magnitude(newPos - new Vector3(0, 1.0f, 0)));
        //Debug.Log("grip:" + Vector3.Magnitude(gripPlace - new Vector3(0, 1.0f, 0)));
    }

    public void updataTargetParams()
    {
        eulerAngles = transform.rotation.eulerAngles / 360f;
        targetPos = transform.position - scene.transform.position; //transform.localPosition;
        targetForward = transform.forward;
        gripPlace = targetPos - 0.15f * targetForward;
    }
}
