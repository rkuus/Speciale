using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGridCreator : MonoBehaviour
{
    public GameObject XY_cube;
    public GameObject ZY_cube;
    public GameObject XZ_cube;
    public GameObject ground;

    public float scaleCube = 0.1f;
    public float robotRange = 2.0f;
    public float[] cubeTriggers;
    void Start()
    {
        // ZY-cube, moves in X
        for (float x = -robotRange; x < robotRange; x += scaleCube)
        {
            ZY_cube.transform.localScale = new Vector3(scaleCube, robotRange, robotRange*2.0f);
            GameObject newCube = Instantiate(ZY_cube, new Vector3(x, robotRange/2.0f, 0.0f) + ground.transform.position, Quaternion.identity);
            newCube.transform.SetParent(this.transform);
        }
        // XY-cube, moves in Z
        for (float z = -robotRange; z < robotRange; z += scaleCube)
        {
            XY_cube.transform.localScale = new Vector3(robotRange*2.0f, robotRange , scaleCube);
            GameObject newCube = Instantiate(XY_cube, new Vector3(0.0f, robotRange / 2.0f, z) + ground.transform.position, Quaternion.identity);
            newCube.transform.SetParent(this.transform);
        }
        // XZ-cube, moves in Y
        for (float y = scaleCube/2.0f; y < robotRange; y += scaleCube)
        {
            XZ_cube.transform.localScale = new Vector3(robotRange * 2.0f, scaleCube, robotRange*2.0f);
            GameObject newCube = Instantiate(XZ_cube, new Vector3(0.0f, y, 0.0f) + ground.transform.position, Quaternion.identity);
            newCube.transform.SetParent(this.transform);
        }
        cubeTriggers = new float[transform.childCount];
        //for (int i = 0; i < transform.childCount; i++)
        //    cubeTriggers[i] = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //voxelCollisions();
    }

    public float[] voxelCollisions()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            cubeScript cube = transform.GetChild(i).GetComponent<cubeScript>();
            if (cube.triggerCounter > 0)
                cubeTriggers[i] = 1.0f;
            else
                cubeTriggers[i] = 0.0f;
        }
        return cubeTriggers;
    }
}
