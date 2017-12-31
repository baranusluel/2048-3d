using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    Vector3 oldPos;
    Vector3 destPos;
    Transform destCube;
    int value = 2;
    public static float moveSpeed;
    bool spawning = true;
    bool merging = false;
    public static float spawnSpeed;
    public bool selfDestruct { get;  set; }    

    Dictionary<int, string> colors = new Dictionary<int, string>()
    {
        {2, "#ecdfc7"},
        {4, "#efcbac"},
        {8, "#f2b179"},
        {16, "#f59563"},
        {32, "#f67c5f"},
        {64, "#f95c30"},
        {128, "#edce68"},
        {256, "#eecd57"},
        {512, "#eec943"},
        {1024, "#eec62c"},
        {2048, "#eec308"}
    };

	void Start ()
    {
        destPos = transform.position;
        oldPos = transform.position;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        selfDestruct = false;
    }

    public void SetValue(int val)
    {
        float size = 0.05f;
        if (val >= 1024)
            size = 0.03f;
        else if (val >= 128)
            size = 0.04f;
        Component[] children = gameObject.GetComponentsInChildren(typeof(TextMesh));
        foreach (Component child in children)
        {
            TextMesh text = (TextMesh)child;
            text.text = val.ToString();
            text.characterSize = size;
            if (val > 2048)
                text.color = Color.white;
        }
        Color col = new Color();
        string colStr = "#000000";
        if (val <= 2048)
            colStr = colors[val];
        //print(colStr);
        ColorUtility.TryParseHtmlString(colStr, out col);
        GetComponent<Renderer>().material.color = col;
        value = val;
    }

    public void SetTarget(Vector3 dest, Transform replace = null)
    {
        destPos = dest;
        destCube = replace;
        spawning = false;
    }
	
	void Update ()
    {
        if (destPos != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime * Mathf.Pow(2, GameBehaviour.moves.Count));
            if ((transform.position - oldPos).magnitude < (transform.position - destPos).magnitude)
            {
                float ratio = (destPos - transform.position).magnitude / (destPos - oldPos).magnitude;
                if (ratio > 0.7)
                    transform.localScale = ratio * new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                float ratio = (transform.position - oldPos).magnitude / (destPos - oldPos).magnitude;
                if (ratio > 0.7)
                    transform.localScale = ratio * new Vector3(1.3f, 1.3f, 1.3f);
            }
        }
        else if (oldPos != transform.position)
        {
            oldPos = transform.position;
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            GameBehaviour.movingCount--;
            if (destCube != null)
            {
                SetValue(2 * value);
                destCube.GetComponent<CubeBehaviour>().selfDestruct = true;
                destCube = null;
                merging = true;
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                if (value == 2048)
                {
                    print("You Won!");
                }
            }
        }
        if (spawning || merging)
        {
            float coeff = (merging ? -0.5f : 1);
            Vector3 newScale = transform.localScale + new Vector3(1, 1, 1) * Time.deltaTime * spawnSpeed * coeff;
            if (coeff * newScale.x <= coeff * 1.3)
                transform.localScale = newScale;
            else
            {
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                spawning = false;
                merging = false;
            }
        }
    }

    void LateUpdate()
    {
        if (selfDestruct)
            Destroy(this.gameObject);
    }
}
