using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingObstacle : MonoBehaviour
{
    public GameObject ground;
    public float outerDiameter = 3f;
    private Rigidbody thisRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();

        Vector2 newDirection = Random.insideUnitCircle.normalized * 0.35f;
        thisRigidbody.velocity = new Vector3(newDirection.x, 0, newDirection.y);
    }

    // Update is called once per frame
    void Update()
    {

        if (new Vector2(transform.localPosition.x, transform.localPosition.z).sqrMagnitude > (outerDiameter) || thisRigidbody.velocity.sqrMagnitude < 0.01f)
            thisRigidbody.velocity = (Random.onUnitSphere-(transform.localPosition * 0.25f)) * 0.35f;

    }

    public void updateDirection()
    {
        Vector2 newDirection = Random.insideUnitCircle.normalized * 0.35f;
        thisRigidbody.velocity = new Vector3(newDirection.x, 0, newDirection.y);
    }
}
