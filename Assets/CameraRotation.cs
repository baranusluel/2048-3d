using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    Vector3 center = new Vector3(3, 3, 3);
    Transform trans;

    public static bool demoMode;
    public static TextMesh[] arrowLabels;
    public static float speed;

    void Start ()
    {
        trans = new GameObject().transform;
    }
	
	void Update ()
    {
        if (demoMode)
        {
            Vector3 newAng = trans.eulerAngles;
            newAng.y += 50f * Time.deltaTime;
            newAng.x = 15 * Mathf.Sin(newAng.y * Mathf.PI / 180) + 30;
            trans.eulerAngles = newAng;
            transform.position = trans.TransformPoint(new Vector3(0, 0, -28)) + center;
            transform.LookAt(center);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newAng = trans.eulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * speed * Time.deltaTime, 0);
            float newX = newAng.x + -Input.GetAxis("Mouse Y") * speed * Time.deltaTime;
            if (newX < 89.9 || newX > 270.1)
                newAng.x = newX;
            trans.eulerAngles = newAng;
            transform.position = trans.TransformPoint(new Vector3(0, 0, -28)) + center;
            transform.LookAt(center);
            foreach (TextMesh lbl in arrowLabels)
            {
                lbl.transform.LookAt(-(transform.position - center) + center);
            }
        }

    }
}
