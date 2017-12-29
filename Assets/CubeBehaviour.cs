using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    public Vector3 destPos { get; set; }
    Vector3 oldPos;
    public float moveSpeed = 1.0f;

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
        }
        Color col = new Color();
        ColorUtility.TryParseHtmlString(colors[val], out col);
        GetComponent<Renderer>().material.color = col;
    }
	
	void Update ()
    {
        if (destPos != transform.position)
        {
            if ((transform.position - oldPos).magnitude < (transform.position - destPos).magnitude)
            {
                if (transform.localScale.x > 1.1)
                    transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                if (transform.localScale.x < 1.3)
                    transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime);
        }
        else if (oldPos != transform.position)
        {
            oldPos = transform.position;
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
	}
}
