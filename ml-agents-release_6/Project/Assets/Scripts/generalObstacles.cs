using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generalObstacles : MonoBehaviour
{
    public GameObject ground;
    public GameObject SafetyZone;

    public float innerDiameter = 0.6f;
    public float outerDiameter = 1.6f;

    public bool isCube = false;

    public bool isCylinder = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            updatePos();
        }
        
    }

    public void updatePos()
    {
        Vector2 newPos;
        LayerMask mask = ~LayerMask.GetMask("floor");
        //Vector3 newPos3D;

        if (isCube)
        {
            //SafetyZone.SetActive(true);
            
            do
            {
                newPos = Random.insideUnitCircle * outerDiameter;
                transform.RotateAround(transform.position, Vector3.up, Random.Range(-180, 180));
                
            } while (Physics.CheckBox(new Vector3(newPos.x + ground.transform.position.x, transform.position.y + 0.01f, newPos.y + ground.transform.position.z), transform.localScale * 0.5f, transform.rotation, mask));

            transform.localPosition = new Vector3(newPos.x, transform.position.y, newPos.y);

            //SafetyZone.SetActive(false);
            return;
        }

        if (isCylinder)
        {
            //SafetyZone.SetActive(true);
            CapsuleCollider thisCollider = GetComponent<CapsuleCollider>();

            do
            {
                newPos = Random.insideUnitCircle * outerDiameter;
                    
            } while (Physics.CheckCapsule(new Vector3(ground.transform.position.x + newPos.x, 2f - thisCollider.radius * transform.localScale.x, ground.transform.position.z + newPos.y), new Vector3(ground.transform.position.x + newPos.x, thisCollider.radius * transform.localScale.x, ground.transform.position.z + newPos.y), (thisCollider.radius * transform.localScale.x), mask));

            transform.localPosition = new Vector3(newPos.x, 1f, newPos.y);

            //SafetyZone.SetActive(false);
            return;
        }
    }
}
