using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameBehaviour : MonoBehaviour
{
    public Transform cube;
    public Transform arrow;
    public Transform grid;
    public bool demoMode = false;
    public float rotationSpeed = 1.0F;
    public float cubeMoveSpeed = 1.0f;
    public enum GenerationModes
    {
        normal, demo, testingWon, testingLost, testingOther
    }
    public GenerationModes generationMode;
    public float startAngle = 45;
    public float startupAnimationSpeed = 1.0F;
    public float cubeSpawnSpeed = 1.0F;

    int[,,] values = new int[4, 4, 4];
    Transform[,,] cubes = new Transform[4, 4, 4];
    public static Queue moves = new Queue();
    public static int movingCount = 0;
    Transform clickedArrow;
    System.Random random = new System.Random();
    public static float sensitivitySlider = 1;
    Slider slider;
    public static bool paused = false;
    GameObject pausePanel;

    void Start()
    {
        CameraRotation.demoMode = demoMode;
        CameraRotation.speed = rotationSpeed;
        CameraRotation.startAngle = startAngle;
        CameraRotation.startupSpeed = startupAnimationSpeed;
        CubeBehaviour.moveSpeed = cubeMoveSpeed;
        CubeBehaviour.spawnSpeed = cubeSpawnSpeed;

        slider = GameObject.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { sliderCallBack(slider.value); });

        pausePanel = GameObject.Find("Panel");
        pausePanel.SetActive(false);

        InitializeGrid();
        if (!demoMode)
            InitializeArrows();
        else
        {
            generationMode = GenerationModes.demo;
            slider.gameObject.transform.parent.gameObject.SetActive(false); // Disable UI Canvas if demoMode
        }
        StartCoroutine(InitializeCubes());
    }

    public IEnumerator InitializeCubes()
    {
        yield return new WaitForSeconds(0.5f);
        switch (generationMode)
        {
            case GenerationModes.normal:
                InitializeCubesNormal();
                break;
            case GenerationModes.demo:
                InitializeCubesDemo();
                break;
            case GenerationModes.testingWon:
                InitializeCubesWon();
                break;
            case GenerationModes.testingLost:
                InitializeCubesLost();
                break;
            case GenerationModes.testingOther:
                InitializeCubesOther();
                break;
        }
        yield return null;
    }

    void InitializeCubesNormal()
    {
        GenerateCube(); GenerateCube();
    }

    void InitializeCubesDemo()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    if (x + y - z < 2)
                    {
                        values[x, y, z] = (int)Mathf.Pow(2, Mathf.RoundToInt((float)(UnityEngine.Random.value * /*11->2048*/ 9 + 0.5)));
                        Transform t = Instantiate(cube, new Vector3(2 * x, 2 * y, 2 * z), Quaternion.identity);
                        cubes[x, y, z] = t;
                        t.GetComponent<CubeBehaviour>().SetValue(values[x, y, z]);
                    }
                }
            }
        }
    }

    void InitializeCubesWon()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    values[x, y, z] = 1024;
                    Transform t = Instantiate(cube, new Vector3(2 * x, 2 * y, 2 * z), Quaternion.identity);
                    cubes[x, y, z] = t;
                    t.GetComponent<CubeBehaviour>().SetValue(values[x, y, z]);
                }
            }
        }
    }

    void InitializeCubesLost()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    values[x, y, z] = (x + y + z) % 2 == 0 ? 2 : 4;
                    Transform t = Instantiate(cube, new Vector3(2 * x, 2 * y, 2 * z), Quaternion.identity);
                    cubes[x, y, z] = t;
                    t.GetComponent<CubeBehaviour>().SetValue(values[x, y, z]);
                }
            }
        }
    }
    void InitializeCubesOther()
    {
        for (int x = 0; x < 1; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    int value = 2;
                    if (z == 0)
                        value = 4;
                    else if (z == 3)
                        value = 8;
                    values[x, y, z] = value;
                    Transform t = Instantiate(cube, new Vector3(2 * x, 2 * y, 2 * z), Quaternion.identity);
                    cubes[x, y, z] = t;
                    t.GetComponent<CubeBehaviour>().SetValue(values[x, y, z]);
                }
            }
        }
    }

    void InitializeArrows()
    {
        #if UNITY_ANDROID || UNITY_IOS
            float scale = 2.0f;
            float offset1 = 4f;
            float offset2 = 2f;
            String[] chars = { "", "", "", "", "", "" };
        #else
            float scale = 1.0f;
            float offset1 = 3.5f;
            float offset2 = 2.5f;
            String[] chars = { "Q", "E", "D", "A", "W", "S" };
        #endif
        Quaternion[] angles = { Quaternion.identity, Quaternion.Euler(180, 0, 0), Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 90), Quaternion.Euler(90, 0, 0), Quaternion.Euler(-90, 0, 0) };
        Vector3[] positions = { new Vector3(offset1, 8, offset2), new Vector3(offset1, -2, offset1), new Vector3(8, offset2, offset2), new Vector3(-2, offset1, offset2), new Vector3(offset1, offset1, 8), new Vector3(offset1, offset2, -2) };
        for (int i = 0; i < 6; i++)
        {
            Transform ar = Instantiate(arrow, positions[i], angles[i]);
            ar.localScale = ar.localScale * scale;
            TextMesh lbl = (TextMesh)ar.GetComponentInChildren(typeof(TextMesh));
            lbl.text = chars[i];
            lbl.transform.Rotate(-angles[i].eulerAngles);
            CameraRotation.arrowLabels[i] = lbl;
        }
    }

    void InitializeGrid()
    {
        Quaternion[] angles = { Quaternion.Euler(180, 0, 0), Quaternion.identity, Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, -90), Quaternion.Euler(-90, 0, 0), Quaternion.Euler(90, 0, 0) };
        Vector3[] positions = { new Vector3(3, 7, 3), new Vector3(3, -1, 3), new Vector3(7, 3, 3), new Vector3(-1, 3, 3), new Vector3(3, 3, 7), new Vector3(3, 3, -1) };
        for (int i = 0; i < 6; i++)
        {
            Transform ar = Instantiate(grid, positions[i], angles[i]);
        }
    }

    void sliderCallBack(float value)
    {
        sensitivitySlider = Mathf.Pow(value, 3 / 2) / Mathf.Pow(5, 3 / 2);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            pausePanel.SetActive(paused);
        }
        if (paused || demoMode)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, ~(1 << 9)).OrderBy(h=>h.distance).ToArray();
            if (hits.Length > 0 && hits[0].transform.tag.Equals("Arrow"))
            {
                clickedArrow = hits[0].transform;
                clickedArrow.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                Color clr = clickedArrow.gameObject.GetComponent<Renderer>().material.color + new Color(0.2f, 0.2f, 0.2f, 0.2f);
                clickedArrow.gameObject.GetComponent<Renderer>().material.color = clr;
            }
        }
        else if (Input.GetMouseButtonUp(0) && clickedArrow != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, ~(1 << 9)).OrderBy(h => h.distance).ToArray();
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
        if (movingCount != 0) // at least one cube has moved
        {
            GenerateCube(); GenerateCube();
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
            if (cubes[(int)target.x, (int)target.y, (int)target.z].GetComponent<CubeBehaviour>().merging)
                return;
            if (!recursion)
                movingCount++;
            cubes[x, y, z].GetComponent<CubeBehaviour>().SetTarget(target * 2, cubes[(int)target.x, (int)target.y, (int)target.z]);
            cubes[(int)target.x, (int)target.y, (int)target.z] = cubes[x, y, z];
            cubes[x, y, z] = null;
            values[(int)target.x, (int)target.y, (int)target.z] = 2 * values[x, y, z];
            values[x, y, z] = 0;
        }
    }

    void GenerateCube()
    {
        List<int[]> coords = FindEmpty();
        if (coords.Count > 0)
        {
            int[] coord = coords[random.Next(coords.Count)];
            int value = UnityEngine.Random.value > 0.1 ? 2 : 4;
            values[coord[0], coord[1], coord[2]] = value;
            Transform t = Instantiate(cube, new Vector3(2 * coord[0], 2 * coord[1], 2 * coord[2]), Quaternion.identity);
            cubes[coord[0], coord[1], coord[2]] = t;
            t.GetComponent<CubeBehaviour>().SetValue(value);
        }
        else
        {
            // check if game over
        }
    }

    List<int[]> FindEmpty()
    {
        List<int[]> coords = new List<int[]>();
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    if (values[x, y, z] == 0)
                    {
                        coords.Add(new int[] { x, y, z} );
                    }
                }
            }
        }
        return coords;
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