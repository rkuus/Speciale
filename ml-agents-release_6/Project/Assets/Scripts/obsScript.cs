using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obsScript : MonoBehaviour
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
        checkRadius = (GetComponent<SphereCollider>().radius * transform.localScale.x) + 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateObsPos()
    {
        Vector3 newPos;
        do
        {
            newPos = Random.onUnitSphere * (Random.value * (outerDiameter - innerDiameter) + innerDiameter);
            newPos += new Vector3(0, 0.05f, 0);
        } while (newPos.y < 0.0f || Vector3.Magnitude(new Vector3(newPos.x, 0, newPos.z)) < innerDSquared || Physics.OverlapSphere(newPos + ground.transform.position, checkRadius, ~0, QueryTriggerInteraction.Ignore).Length > 0); ;

        transform.localPosition = newPos;
    }
}
