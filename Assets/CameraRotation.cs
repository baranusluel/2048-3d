using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    Vector3 center = new Vector3(3, 3, 3);
    Transform trans;
    float distance = 28;
    Camera cam;
    float width;
    float height;
    int startingDist = 18;

    public static bool paused = false;
    public static bool demoMode;
    public static TextMesh[] arrowLabels = new TextMesh[6];
    public static float speed;
    public static float startAngle;
    public static float startupSpeed;

    void Start ()
    {
        trans = new GameObject().transform;
        cam = transform.GetComponent<Camera>();
        AdjustDistance();

        if (!demoMode)
            StartCoroutine(StartupAnimation());
        else
            StartCoroutine(DemoAnimation());
    }

    void TransformCamera(Vector3 newAng, bool arrows = false)
    {
        trans.eulerAngles = newAng;
        transform.position = trans.TransformPoint(new Vector3(0, 0, -distance)) + center;
        transform.LookAt(center);
        if (!arrows) return;
        foreach (TextMesh lbl in arrowLabels)
        {
            lbl.transform.up = transform.up;
            lbl.transform.forward = transform.forward;
        }
    }

    void AdjustDistance()
    {
        width = Screen.width; height = Screen.height;

        if (!demoMode)
        {
            if (height > width)
            {
                GameBehaviour.RotateDisplay(false);
                if (height * 0.7 > width)
                    distance = startingDist / cam.aspect * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                else
                    distance = startingDist * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }
            else
            {
                GameBehaviour.RotateDisplay(true);
                if (height < width * 0.7)
                    distance = startingDist * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                else
                    distance = startingDist / cam.aspect * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }
        }
        else
        {
            if (height > width)
                distance = startingDist / cam.aspect * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            else
                distance = startingDist * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        TransformCamera(trans.eulerAngles);
    }

    IEnumerator StartupAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 newAng = trans.eulerAngles;
        while (Mathf.Abs(newAng.y) < Mathf.Abs(startAngle))
        {
            float delta = newAng.y * (newAng.y - startAngle) / -Mathf.Pow(startAngle/2, 2) + 0.1f;
            newAng.y += delta * startupSpeed * Mathf.Sign(startAngle) * Time.deltaTime;
            newAng.x = 30 * (1-Mathf.Cos(3 * newAng.y * Mathf.PI / 180));
            TransformCamera(newAng, true);
            yield return null;
        }
    }

    IEnumerator DemoAnimation()
    {
        Vector3 newAng = trans.eulerAngles;
        newAng.x = 15;
        TransformCamera(newAng);
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            newAng.y += 25f * Time.deltaTime;
            newAng.x = 15 + 15 * Mathf.Sin(newAng.y * Mathf.PI / 180);
            TransformCamera(newAng);
            yield return null;
        }
    }

    void Update ()
    {
        if (width != Screen.width || height != Screen.height)
            AdjustDistance();

        if (!demoMode && !paused && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase != TouchPhase.Moved) return;
                deltaX = t.deltaPosition.x * Time.deltaTime / t.deltaTime;
                deltaY = t.deltaPosition.y * Time.deltaTime / t.deltaTime;
            }
            float dpi = Screen.dpi != 0 ? Screen.dpi : 96;
            Vector3 newAng = trans.eulerAngles + new Vector3(0, deltaX * speed * (float)GameBehaviour.sensitivitySlider / dpi / (Screen.width + Screen.height), 0);
            float newX = newAng.x - (deltaY * speed * (float)GameBehaviour.sensitivitySlider / dpi / (Screen.width + Screen.height));
            if (newX < 89.9 || newX > 270.1)
                newAng.x = newX;
            TransformCamera(newAng, true);
        }

    }
}
