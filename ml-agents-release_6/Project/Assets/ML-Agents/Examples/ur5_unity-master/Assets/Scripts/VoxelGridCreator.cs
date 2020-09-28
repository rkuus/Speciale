using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGridCreator : MonoBehaviour
{
    public GameObject collsionCube;

    public float scaleCube = 1.0f;
    public float robotRange = 10.0f;
    public int[] cubeTriggers;
    void Start()
    {
        for (float x = -robotRange; x < robotRange; x += scaleCube)
        {
            for (float y = 1.0f + (0.5f * scaleCube); y < robotRange; y += scaleCube)
            {
                for (float z = -robotRange; z < robotRange; z += scaleCube)
                {
                    if (Vector3.Magnitude(new Vector3(x, y, z)) > robotRange)
                        continue;
                    collsionCube.transform.localScale = new Vector3(scaleCube, scaleCube, scaleCube);
                    GameObject newCube = Instantiate(collsionCube, new Vector3(x, y, z), Quaternion.identity);
                    newCube.transform.SetParent(this.transform);
                }
            }
        }
        cubeTriggers = new int[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            cubeTriggers[i] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0;i<transform.childCount;i++)
        {
            cubeScript cube = transform.GetChild(i).GetComponent<cubeScript>();
            if (cube.triggerCounter > 0)
                cubeTriggers[i] = 1;
            else
                cubeTriggers[i] = 0;
        }
    }
}
