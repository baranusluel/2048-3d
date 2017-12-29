using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{

    public Transform cube;
    public Transform arrow;
    
    int[,,] values = new int[4, 4, 4];
    Transform[,,] cubes = new Transform[4, 4, 4];

    // Use this for initialization
    void Start()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    if (x + y - z < 2)
                    {
                        values[x, y, z] = (int)Mathf.Pow(2, Mathf.RoundToInt((float)(Random.value * 11 + 0.5)));
                        Transform t = Instantiate(cube, new Vector3(2 * x, 2 * y, 2 * z), Quaternion.identity);
                        cubes[x, y, z] = t;
                        t.GetComponent<CubeBehaviour>().SetValue(values[x, y, z]);
                    }
                }
            }
        }
        Instantiate(arrow, new Vector3(3.5f, 8f, 2.5f), Quaternion.identity);
        Instantiate(arrow, new Vector3(3.5f, -2f, 3.5f), Quaternion.Euler(180,0,0));
        Instantiate(arrow, new Vector3(8f, 2.5f, 2.5f), Quaternion.Euler(0, 0, -90));
        Instantiate(arrow, new Vector3(-2f, 3.5f, 2.5f), Quaternion.Euler(0, 0, 90));
        Instantiate(arrow, new Vector3(3.5f, 3.5f, 8f), Quaternion.Euler(90, 0, 0));
        Instantiate(arrow, new Vector3(3.5f, 2.5f, -2f), Quaternion.Euler(-90, 0, 0));
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0 && hits[0].transform.tag.Equals("Arrow"))
            {
                print(hits[0].transform.TransformDirection(new Vector3(0, 1, 0)));
                MoveCubes(hits[0].transform.TransformDirection(new Vector3(0, 1, 0)));
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveCubes(new Vector3(0, 0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveCubes(new Vector3(0, 0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveCubes(new Vector3(-1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveCubes(new Vector3(1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveCubes(new Vector3(0, 1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            MoveCubes(new Vector3(0, -1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
        else
        {
            // show help menu listing possible keys
        }
    }

    void MoveCubes(Vector3 dirVec)
    {
        int[] dir = { (int)dirVec.x, (int)dirVec.y, (int)dirVec.z };
        int increment = -(dir[0] + dir[1] + dir[2]);
        int startVal = (increment == 1) ? 0 : 3;
        int endVal = increment * (4 - startVal);
        for (int x = startVal; x != endVal; x += increment)
        {
            for (int y = startVal; y != endVal; y += increment)
            {
                for (int z = startVal; z != endVal; z += increment)
                {
                    MoveCubeRecursive(x, y, z, dir);
                }
            }
        }
    }

    void MoveCubeRecursive(int x, int y, int z, int[] dir)
    {
        if (values[x, y, z] == 0) return;
        Vector3 target = new Vector3(x + dir[0], y + dir[1], z + dir[2]);
        if (target.x < 0 || target.y < 0 || target.z < 0 || target.x > 3 || target.y > 3 || target.z > 3) return;
        if (values[(int)target.x, (int)target.y, (int)target.z] == 0)
        {
            cubes[x, y, z].GetComponent<CubeBehaviour>().destPos = target * 2;
            cubes[(int)target.x, (int)target.y, (int)target.z] = cubes[x, y, z];
            cubes[x, y, z] = null;
            values[(int)target.x, (int)target.y, (int)target.z] = values[x, y, z];
            values[x, y, z] = 0;
        }
        MoveCubeRecursive((int)target.x, (int)target.y, (int)target.z, dir);
    }

    void OnDrawGizmos()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    Gizmos.DrawWireCube(new Vector3(2 * x, 2 * y, 2 * z), new Vector3(1.25f, 1.25f, 1.25f));
                }
            }
        }
    }
}
