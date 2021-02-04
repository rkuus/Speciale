using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poleScript : MonoBehaviour
{

    public GameObject ground;

    public float innerDiameter = 0.6f;
    public float outerDiameter = 1.6f;

    private float innerDSquared = 0.0f;

    private float checkRadius = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        innerDSquared = innerDiameter * innerDiameter;
        checkRadius = (GetComponent<CapsuleCollider>().radius * transform.localScale.x) + 0.25f;
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    updateObsPos();
        //}
    }

    public void updateObsPos()
    {
        Vector2 newPos;
        Vector3 newPos3D;
        RaycastHit hit;
        do
        {
            newPos = Random.insideUnitCircle * outerDiameter;
            newPos3D = new Vector3(newPos.x, 2.01f, newPos.y);

        } while (Physics.SphereCast(ground.transform.position + newPos3D, checkRadius, -transform.up, out hit, 2f - checkRadius) || (Vector2.SqrMagnitude(newPos) < innerDSquared));
        newPos3D = new Vector3(newPos.x, 1, newPos.y);
        transform.localPosition = newPos3D;
    }
}
