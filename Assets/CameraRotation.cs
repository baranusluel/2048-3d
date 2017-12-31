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
    public static float startAngle;
    public static float startupSpeed;

    void Start ()
    {
        trans = new GameObject().transform;
        if (!demoMode)
            StartCoroutine(StartupAnimation());
        else
            StartCoroutine(DemoAnimation());
    }

    IEnumerator StartupAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 newAng = trans.eulerAngles;
        while (Mathf.Abs(newAng.y) < Mathf.Abs(startAngle))
        {
            float delta = newAng.y * (newAng.y - startAngle) / -Mathf.Pow(startAngle/2, 2) + 0.1f;
            newAng.y += delta * startupSpeed * Mathf.Sign(startAngle);
            newAng.x = 30 * (1-Mathf.Cos(3 * newAng.y * Mathf.PI / 180));
            trans.eulerAngles = newAng;
            transform.position = trans.TransformPoint(new Vector3(0, 0, -26)) + center;
            transform.LookAt(center);
            foreach (TextMesh lbl in arrowLabels)
            {
                lbl.transform.up = transform.up;
                lbl.transform.forward = transform.forward;
            }
            yield return null;
        }
    }

    IEnumerator DemoAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 newAng = trans.eulerAngles;
        while (true)
        {
            newAng.y += 50f * Time.deltaTime;
            newAng.x = 15 * Mathf.Sin(newAng.y * Mathf.PI / 180);
            trans.eulerAngles = newAng;
            transform.position = trans.TransformPoint(new Vector3(0, 0, -26)) + center;
            transform.LookAt(center);
            yield return null;
        }
    }

    void Update ()
    {
        if (!demoMode && Input.GetMouseButton(0))
        {
            Vector3 newAng = trans.eulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * speed * Time.deltaTime, 0);
            float newX = newAng.x + -Input.GetAxis("Mouse Y") * speed * Time.deltaTime;
            if (newX < 89.9 || newX > 270.1)
                newAng.x = newX;
            trans.eulerAngles = newAng;
            transform.position = trans.TransformPoint(new Vector3(0, 0, -26)) + center;
            transform.LookAt(center);
            foreach (TextMesh lbl in arrowLabels)
            {
                lbl.transform.up = transform.up;
                lbl.transform.forward = transform.forward;
            }
        }

    }
}
