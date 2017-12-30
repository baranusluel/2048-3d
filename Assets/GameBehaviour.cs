using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public Transform cube;
    public Transform arrow;
    public bool demoMode = false;
    public float rotationSpeed = 1.0F;
    public float cubeMoveSpeed = 1.0f;

    int[,,] values = new int[4, 4, 4];
    Transform[,,] cubes = new Transform[4, 4, 4];
    Queue moves = new Queue();
    public static int movingCount = 0;
    Transform clickedArrow;

    void Start()
    {
        CameraRotation.demoMode = demoMode;
        CameraRotation.speed = rotationSpeed;
        CubeBehaviour.moveSpeed = cubeMoveSpeed;

        if (!demoMode)
            InitializeArrows();
        InitializeCubes();
    }

    void InitializeCubes()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    if (x + y - z < 2)
                    {
                        values[x, y, z] = (int)Mathf.Pow(2, Mathf.RoundToInt((float)(Random.value * /*11->2048*/ 9 + 0.5)));
                        Transform t = Instantiate(cube, new Vector3(2 * x, 2 * y, 2 * z), Quaternion.identity);
                        cubes[x, y, z] = t;
                        t.GetComponent<CubeBehaviour>().SetValue(values[x, y, z]);
                    }
                }
            }
        }
    }

    void InitializeArrows()
    {
        Quaternion[] angles = { Quaternion.identity, Quaternion.Euler(180, 0, 0), Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 90), Quaternion.Euler(90, 0, 0), Quaternion.Euler(-90, 0, 0) };
        Transform ar0 = Instantiate(arrow, new Vector3(3.5f, 8f, 2.5f), angles[0]);
        TextMesh lbl0 = (TextMesh)ar0.GetComponentInChildren(typeof(TextMesh));
        Transform ar1 = Instantiate(arrow, new Vector3(3.5f, -2f, 3.5f), angles[1]);
        TextMesh lbl1 = (TextMesh)ar1.GetComponentInChildren(typeof(TextMesh));
        lbl1.text = "E"; lbl1.transform.Rotate(-angles[1].eulerAngles);
        Transform ar2 = Instantiate(arrow, new Vector3(8f, 2.5f, 2.5f), angles[2]);
        TextMesh lbl2 = (TextMesh)ar2.GetComponentInChildren(typeof(TextMesh));
        lbl2.text = "D"; lbl2.transform.Rotate(-angles[2].eulerAngles);
        Transform ar3 = Instantiate(arrow, new Vector3(-2f, 3.5f, 2.5f), angles[3]);
        TextMesh lbl3 = (TextMesh)ar3.GetComponentInChildren(typeof(TextMesh));
        lbl3.text = "A"; lbl3.transform.Rotate(-angles[3].eulerAngles);
        Transform ar4 = Instantiate(arrow, new Vector3(3.5f, 3.5f, 8f), angles[4]);
        TextMesh lbl4 = (TextMesh)ar4.GetComponentInChildren(typeof(TextMesh));
        lbl4.text = "W"; lbl4.transform.Rotate(-angles[4].eulerAngles);
        Transform ar5 = Instantiate(arrow, new Vector3(3.5f, 2.5f, -2f), angles[5]);
        TextMesh lbl5 = (TextMesh)ar5.GetComponentInChildren(typeof(TextMesh));
        lbl5.text = "S"; lbl5.transform.Rotate(-angles[5].eulerAngles);
        CameraRotation.arrowLabels = new TextMesh[] { lbl0, lbl1, lbl2, lbl3, lbl4, lbl5 };
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0 && hits[0].transform.tag.Equals("Arrow"))
            {
                clickedArrow = hits[0].transform;
                print(clickedArrow.localScale);
                clickedArrow.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                Color clr = clickedArrow.gameObject.GetComponent<Renderer>().material.color + new Color(0.2f, 0.2f, 0.2f, 0.2f);
                clickedArrow.gameObject.GetComponent<Renderer>().material.color = clr;
            }
        }
        else if (Input.GetMouseButtonUp(0) && clickedArrow != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0 && hits[0].transform.tag.Equals("Arrow") && hits[0].transform == clickedArrow)
            {
                moves.Enqueue(clickedArrow.TransformDirection(new Vector3(0, 1, 0)));
            }
            clickedArrow.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Color clr = clickedArrow.gameObject.GetComponent<Renderer>().material.color - new Color(0.2f, 0.2f, 0.2f, 0.2f);
            clickedArrow.gameObject.GetComponent<Renderer>().material.color = clr;
            clickedArrow = null;
        }

        if (Input.GetKeyDown(KeyCode.W))
            moves.Enqueue(new Vector3(0, 0, 1));
        else if (Input.GetKeyDown(KeyCode.S))
            moves.Enqueue(new Vector3(0, 0, -1));
        else if (Input.GetKeyDown(KeyCode.A))
            moves.Enqueue(new Vector3(-1, 0, 0));
        else if (Input.GetKeyDown(KeyCode.D))
            moves.Enqueue(new Vector3(1, 0, 0));
        else if (Input.GetKeyDown(KeyCode.Q))
            moves.Enqueue(new Vector3(0, 1, 0));
        else if (Input.GetKeyDown(KeyCode.E))
            moves.Enqueue(new Vector3(0, -1, 0));
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // pause / quit menu
        }
        else
        {
            // show help menu listing possible keys
        }

        MoveCubes();
    }

    void MoveCubes()
    {
        if (movingCount != 0) return;
        if (moves.Count == 0) return;
        Vector3 dirVec = (Vector3)moves.Dequeue();
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

    void MoveCubeRecursive(int x, int y, int z, int[] dir, bool recursion = false)
    {
        if (values[x, y, z] == 0) return;
        Vector3 target = new Vector3(x + dir[0], y + dir[1], z + dir[2]);
        if (target.x < 0 || target.y < 0 || target.z < 0 || target.x > 3 || target.y > 3 || target.z > 3) return;
        if (values[(int)target.x, (int)target.y, (int)target.z] == 0)
        {
            if (!recursion)
                movingCount++;
            cubes[x, y, z].GetComponent<CubeBehaviour>().SetTarget(target * 2);
            cubes[(int)target.x, (int)target.y, (int)target.z] = cubes[x, y, z];
            cubes[x, y, z] = null;
            values[(int)target.x, (int)target.y, (int)target.z] = values[x, y, z];
            values[x, y, z] = 0;
            MoveCubeRecursive((int)target.x, (int)target.y, (int)target.z, dir, true);
        }
        else if (values[(int)target.x, (int)target.y, (int)target.z] == values[x, y, z])
        {
            if (!recursion)
                movingCount++;
            cubes[x, y, z].GetComponent<CubeBehaviour>().SetTarget(target * 2, cubes[(int)target.x, (int)target.y, (int)target.z]);
            cubes[(int)target.x, (int)target.y, (int)target.z] = cubes[x, y, z];
            cubes[x, y, z] = null;
            values[(int)target.x, (int)target.y, (int)target.z] = 2 * values[x, y, z];
            values[x, y, z] = 0;
        }
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

    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
